/*global
    piranha
*/

piranha.workflowedit = new Vue({
    el: "#workflowedit",
    data: {
        loading: true,
        id: null,
        title: "",
        description: "",
        stages: [],
        saveUrl: piranha.baseUrl + "manager/api/workflow/save",
        originalTitle: null
    },
    methods: {
        bind: function (result) {
            this.id = result.id;
            this.title = result.title;
            this.originalTitle = result.title;
            this.description = result.description;
            this.stages = result.stages || [];
            
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
                name: "Draft",
                description: "Initial draft stage"
            });
            
            this.stages.push({
                id: piranha.utils.generateId(),
                name: "Review",
                description: "Content review stage"
            });
            
            this.stages.push({
                id: piranha.utils.generateId(),
                name: "Published",
                description: "Final published stage"
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
            
            // Make sure all stages have names
            for (var i = 0; i < self.stages.length; i++) {
                if (!self.stages[i].name) {
                    piranha.notifications.push({
                        body: "Please enter a name for all stages",
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
                stages: self.stages
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
                name: "",
                description: ""
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
        }
    },
    created: function () {
        // Initialize utils (if needed)
        if (!piranha.utils) {
            piranha.utils = {
                generateId: function () {
                    return Math.random().toString(36).substring(2, 15) +
                        Math.random().toString(36).substring(2, 15);
                }
            };
        }
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

// Register utils if needed
if (!piranha.utils) {
    piranha.utils = {
        generateId: function () {
            return Math.random().toString(36).substring(2, 15) +
                Math.random().toString(36).substring(2, 15);
        }
    };
}
