/*global
    piranha
*/

// Initialize utils before creating the Vue component
if (!piranha.utils) {
    piranha.utils = {
        generateId: function () {
            return Math.random().toString(36).substring(2, 15) +
                Math.random().toString(36).substring(2, 15);
        }
    };
}

piranha.workflowedit = new Vue({
    el: "#workflowedit",
    data: {
        loading: true,
        id: null,
        title: "",
        description: "",
        stages: [], // Stages will have { id, title, description }
        availableRoles: [],
        saveUrl: piranha.baseUrl + "manager/api/workflow/save",
        rolesUrl: piranha.baseUrl + "manager/api/workflow/roles",
        originalTitle: null
    },
    methods: {
        bind: function (result) {
            this.id = result.id;
            this.title = result.title;
            this.originalTitle = result.title;
            this.description = result.description;
            // Convert stages and roles from API format to UI format
            this.stages = (result.stages || []).map(function(stage) {
                return {
                    id: stage.id,
                    title: stage.title,
                    description: stage.description,
                    roleIds: (stage.roles || []).map(function(role) {
                        return role.roleId;
                    }) || [] // Ensure this is always an array
                };
            });
            
            this.loading = false;
        },
        load: function (id) {
            var self = this;
            
            if (id) {
                // Store the id
                self.id = id;
                
                fetch(piranha.baseUrl + "manager/api/workflow/" + id)
                    .then(function (response) { 
                        if (!response.ok) {
                            throw new Error("Response status: " + response.status);
                        }
                        return response.json();
                    })
                    .then(function (result) {
                        self.bind(result);
                    })
                    .catch(function (error) {
                        console.log("Error loading workflow:", error);
                        
                        piranha.notifications.push({
                            body: "Failed to load workflow. Please try again.",
                            type: "danger",
                            hide: true
                        });
                    });
            }
        },
        create: function () {
            // Reset the data
            this.id = null;
            this.title = "";
            this.description = "";
            this.stages = [];
            
            // Add default stages for a new workflow
            this.stages.push({
                id: piranha.utils.generateId(),
                title: "Draft", // Changed from name to title
                description: "Initial draft stage",
                roleIds: [] // Ensure this is always an array
            });
            
            this.stages.push({
                id: piranha.utils.generateId(),
                title: "Review", // Changed from name to title
                description: "Content review stage",
                roleIds: [] // Ensure this is always an array
            });
            
            this.stages.push({
                id: piranha.utils.generateId(),
                title: "Published", // Changed from name to title
                description: "Final published stage",
                roleIds: [] // Ensure this is always an array
            });
            
            this.loading = false;
        },
        save: function () {
            var self = this;
            
            // Validate form
            if (self.title === "") {
                piranha.notifications.push({
                    body: "Please enter a title for the workflow",
                    type: "danger",
                    hide: true
                });
                return;
            }
            
            if (self.stages.length === 0) {
                piranha.notifications.push({
                    body: "Please add at least one stage to the workflow",
                    type: "danger",
                    hide: true
                });
                return;
            }
            
            // Make sure all stages have titles
            for (var i = 0; i < self.stages.length; i++) {
                if (!self.stages[i].title) { // Changed from name to title
                    piranha.notifications.push({
                        body: "Please enter a title for all stages", // Changed from name to title
                        type: "danger",
                        hide: true
                    });
                    return;
                }
            }
            
            self.loading = true;
            
            var model = {
                id: self.id,
                title: self.title,
                description: self.description,
                stages: self.stages.map(function(stage) {
                    return {
                        id: stage.id,
                        title: stage.title,
                        description: stage.description,
                        roles: (stage.roleIds || []).map(function(roleId) {
                            return {
                                roleId: roleId
                                // Let the backend handle workflowStageId assignment
                            };
                        })
                    };
                })
            };
            
            fetch(self.saveUrl, {
                method: "post",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(model)
            })
            .then(function (response) {
                if (!response.ok) {
                    return response.json().then(function (result) {
                        throw result;
                    });
                }
                return response.json();
            })
            .then(function (result) {
                // Update the state
                self.id = result.id;
                self.originalTitle = result.title;
                
                piranha.notifications.push({
                    body: "The workflow has been saved",
                    type: "success",
                    hide: true
                });
                
                self.loading = false;
                
                // If this was a new workflow, redirect to edit view
                if (!model.id) {
                    window.location = piranha.baseUrl + "manager/workflow/edit/" + result.id;
                }
            })
            .catch(function (error) {
                console.log("Error saving workflow:", error);
                
                piranha.notifications.push({
                    body: error.body || "An error occurred while saving the workflow",
                    type: "danger",
                    hide: true
                });
                
                self.loading = false;
            });
        },
        addStage: function () {
            this.stages.push({
                id: piranha.utils.generateId(),
                title: "", // Changed from name to title
                description: "",
                roleIds: [] // Ensure this is always an array
            });
        },
        removeStage: function (index) {
            this.stages.splice(index, 1);
        },
        moveStageUp: function (index) {
            if (index > 0) {
                const stage = this.stages[index];
                this.stages.splice(index, 1);
                this.stages.splice(index - 1, 0, stage);
            }
        },
        moveStageDown: function (index) {
            if (index < this.stages.length - 1) {
                const stage = this.stages[index];
                this.stages.splice(index, 1);
                this.stages.splice(index + 1, 0, stage);
            }
        },
        updateStageRoles: function (stage, event) {
            // This method is called when role checkboxes are changed
            // Vue's v-model will automatically handle the updates
        },
        toggleStageRole: function (stage, roleId, event) {
            // Initialize roleIds array if it doesn't exist
            if (!stage.roleIds) {
                stage.roleIds = [];
            }
            
            if (event.target.checked) {
                // Add role if not already present
                if (!stage.roleIds.includes(roleId)) {
                    stage.roleIds.push(roleId);
                }
            } else {
                // Remove role if present
                const index = stage.roleIds.indexOf(roleId);
                if (index > -1) {
                    stage.roleIds.splice(index, 1);
                }
            }
        },
        loadRoles: function () {
            var self = this;
            
            fetch(this.rolesUrl)
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Failed to load roles');
                    }
                    return response.json();
                })
                .then(function (roles) {
                    self.availableRoles = roles;
                })
                .catch(function (error) {
                    console.log("Error loading roles:", error);
                    piranha.notifications.push({
                        body: "Failed to load available roles",
                        type: "warning",
                        hide: true
                    });
                });
        }
    },
    created: function () {
        // Vue component initialization
    },
    mounted: function () {
        // Load available roles when component is mounted
        this.loadRoles();
    },
    updated: function () {
        // Initialize sortable elements after the DOM has been updated
        if (!this.loading) {
            var self = this;
            
            // Use the standalone sortable instead of jQuery UI sortable
            sortable(".sortable", {
                handle: ".handle",
                items: ".sortable-item",
                placeholderClass: "sortable-placeholder"
            });
            
            // Add event listener for the end of sorting
            document.querySelector(".sortable").addEventListener("sortupdate", function(e) {
                var oldIndex = e.detail.origin.index;
                var newIndex = e.detail.destination.index;
                
                if (oldIndex !== newIndex) {
                    var stage = self.stages.splice(oldIndex, 1)[0];
                    self.stages.splice(newIndex, 0, stage);
                }
            });
            
            // Set initial indexes (for reference only)
            var items = document.querySelectorAll(".sortable-item");
            for (var i = 0; i < items.length; i++) {
                items[i].setAttribute("data-index", i);
            }
        }
    }
});
