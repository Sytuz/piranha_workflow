/*global
    piranha, Vue, go, $
*/

piranha.workflow = new Vue({
    el: "#workflow",
    data: {
        items: [],
        loading: true,
        loadingFailed: false,
        selectedWorkflow: null,
        searchTerm: "",
        goJsDiagram: null, // Holds the GoJS Diagram instance
        newWorkflowTitle: "",
        newWorkflowDescription: ""
    },
    computed: {
        filteredItems: function() {
            if (!this.searchTerm) {
                return this.items;
            }
            
            const term = this.searchTerm.toLowerCase();
            return this.items.filter(function(workflow) {
                return workflow.title.toLowerCase().includes(term) || 
                       (workflow.description && workflow.description.toLowerCase().includes(term));
            });
        },
    },
    methods: {
        formatDate: function (dateString) {
            if (!dateString) {
                return "";
            }
            try {
                var date = new Date(dateString);
                var day = ('0' + date.getDate()).slice(-2);
                var month = ('0' + (date.getMonth() + 1)).slice(-2);
                var year = date.getFullYear();
                return day + '/' + month + '/' + year;
            } catch (e) {
                console.warn("Could not parse date:", dateString);
                return dateString;
            }
        },
        initGoJsDiagram: function() {
            if (!this.$refs.goJsDiagramDiv) {
                // The div for the diagram is not currently in the DOM (e.g., due to v-if).
                // If a diagram instance exists, it's now orphaned. Clean it up.
                if (this.goJsDiagram) {
                    console.warn("GoJS init: Target div (goJsDiagramDiv) not found in refs, but a GoJS Diagram instance exists. Cleaning up old instance.");
                    this.goJsDiagram.div = null; // Allow GoJS to release resources
                    this.goJsDiagram = null;
                }
                return; // Cannot initialize without the div.
            }

            if (this.goJsDiagram) {
                // A GoJS Diagram instance already exists.
                // Check if it's still attached to the *current* DOM element referenced by goJsDiagramDiv.
                if (this.goJsDiagram.div === this.$refs.goJsDiagramDiv) {
                    // Already initialized and attached to the correct div. Nothing more to do for init.
                    return;
                } else {
                    // The existing diagram instance is attached to an old, now-replaced DOM element.
                    console.warn("GoJS init: GoJS Diagram instance exists but is attached to an old DOM element. Re-initializing on the new div.");
                    this.goJsDiagram.div = null; // Clean up the old diagram's association
                    this.goJsDiagram = null;     // Force re-creation of the diagram object
                }
            }

            // If we reach here, this.goJsDiagram is null, so we need to create a new instance.

            // Ensure GoJS is loaded
            if (typeof go === 'undefined') {
                console.error("GoJS library is not loaded.");
                return;
            }

            // If CSS height isn't applied in time, force it.
            if (this.$refs.goJsDiagramDiv.offsetHeight < 10) { 
                 console.warn("GoJS init (pre-create): goJsDiagramDiv has no significant height. Forcing height.", this.$refs.goJsDiagramDiv.offsetHeight);
                 this.$refs.goJsDiagramDiv.style.height = "400px";
            }

            console.log("GoJS init: Initializing new Diagram instance. Div height now:", this.$refs.goJsDiagramDiv.offsetHeight);
            const $go = go.GraphObject.make; // Renamed to avoid conflict with jQuery's $

            this.goJsDiagram = $go(go.Diagram, this.$refs.goJsDiagramDiv, {
                initialContentAlignment: go.Spot.Center,
                initialScale: 0.95, // Set initial zoom level
                layout: $go(go.LayeredDigraphLayout, { 
                    direction: 0, 
                    layerSpacing: 50,
                    columnSpacing: 30 
                }),
                "undoManager.isEnabled": false,
                "animationManager.isEnabled": false,
                "allowMove": false,
                "allowCopy": false,
                "allowDelete": false,
                "allowInsert": false,
            });

            // Define the Node template
            this.goJsDiagram.nodeTemplate =
                $go(go.Node, "Auto",
                    { 
                        locationSpot: go.Spot.Center,
                        fromSpot: go.Spot.RightCenter, 
                        toSpot: go.Spot.LeftCenter,     
                        selectionObjectName: "PANEL",
                        toolTip: $go(go.Adornment, "Auto",
                                   $go(go.TextBlock, { margin: 4 }, new go.Binding("text", "description"))
                                 )
                    },
                    new go.Binding("layerName", "isPublished", function(is) { return is ? "Foreground" : ""; }),
                    $go(go.Shape, "RoundedRectangle",
                        { fill: "white", stroke: "#BBB", strokeWidth: 1, portId: "" },
                        new go.Binding("fill", "isPublished", function(is) { return is ? "#E6FFED" : "#FFFFFF"; }),
                        new go.Binding("stroke", "isPublished", function(is) { return is ? "#4CAF50" : "#BBB"; }),
                        new go.Binding("strokeWidth", "isPublished", function(is) { return is ? 2 : 1; })
                    ),
                    $go(go.Panel, "Vertical", { margin: 10, defaultAlignment: go.Spot.Left },
                        $go(go.TextBlock,
                            { font: "bold 10pt sans-serif", stroke: "#333", margin: new go.Margin(0, 0, 4, 0) },
                            new go.Binding("text", "title")
                        ),
                        $go(go.TextBlock,
                            { font: "9pt sans-serif", stroke: "#555", wrap: go.TextBlock.WrapDesiredSize, width: 130 },
                            new go.Binding("text", "description", function(d) { return piranha.workflow.truncateDescription(d, 50); })
                        )
                    )
                );

            // Define the Link template
            this.goJsDiagram.linkTemplate =
                $go(go.Link,
                    { 
                        routing: go.Link.AvoidsNodes, 
                        corner: 10, 
                        curve: go.Link.JumpOver,
                        reshapable: false, 
                        resegmentable: false,
                        relinkableFrom: false,
                        relinkableTo: false,
                        selectionAdorned: false 
                    },
                    $go(go.Shape, { strokeWidth: 2, stroke: "#555" }),
                    $go(go.Shape, { toArrow: "Standard", fill: "#555", stroke: null, scale: 1.5 })
                );
        },
        updateGoJsModel: function() {
            if (!this.goJsDiagram) {
                // This should ideally not be hit if initGoJsDiagram is called correctly before updateGoJsModel.
                // initGoJsDiagram is responsible for ensuring a valid diagram instance if the div exists.
                console.error("GoJS updateGoJsModel: Diagram instance is not available. initGoJsDiagram might have failed or was not called when the div was ready.");
                return;
            }

            if (!this.selectedWorkflow || !this.selectedWorkflow.stages || this.selectedWorkflow.stages.length === 0) {
                console.log("GoJS updateGoJsModel: No selected workflow or no stages, clearing model.");
                this.goJsDiagram.model = new go.GraphLinksModel([], []);
                this.goJsDiagram.requestUpdate(); // Ensure view clears
                return;
            }

            const nodeDataArray = (this.selectedWorkflow.stages || []).map(stage => ({
                key: stage.id,
                title: stage.title,
                description: stage.description,
                isPublished: stage.isPublished,
            }));

            const linkDataArray = (this.selectedWorkflow.relations || []).map(rel => ({
                from: rel.sourceStageId,
                to: rel.targetStageId
            }));

            console.log("GoJS updateGoJsModel: Nodes:", nodeDataArray.length, "Links:", linkDataArray.length, "for workflow:", this.selectedWorkflow.id, "Enabled:", this.selectedWorkflow.isEnabled);

            this.goJsDiagram.model = new go.GraphLinksModel(nodeDataArray, linkDataArray);
            this.goJsDiagram.requestUpdate();
        },
        load: function() {
            var self = this;
            self.loading = true;
            
            return fetch(piranha.baseUrl + "manager/api/workflow/list")
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.items = result;
                    
                    if (self.filteredItems && self.filteredItems.length > 0) {
                        var currentSelectedStillVisible = false;
                        if (self.selectedWorkflow) {
                            currentSelectedStillVisible = self.filteredItems.some(item => item.id === self.selectedWorkflow.id);
                        }
                        if (!self.selectedWorkflow || !currentSelectedStillVisible) {
                           self.selectedWorkflow = self.filteredItems[0];
                        }
                    } else {
                        self.selectedWorkflow = null;
                    }
                    
                    self.loading = false;
                    self.loadingFailed = false;
                    self.$nextTick(() => {
                        if (self.selectedWorkflow && self.selectedWorkflow.stages && self.selectedWorkflow.stages.length > 0) {
                            // initGoJsDiagram will only create the diagram once.
                            self.initGoJsDiagram(); 
                            // updateGoJsModel will then set the model and request update.
                            self.updateGoJsModel(); 
                        } else if (self.goJsDiagram) {
                            self.updateGoJsModel(); // This will clear the model
                        }
                    });
                    return result;
                })
                .catch(function (error) {
                    console.error("Error loading workflows:", error);
                    self.loading = false;
                    self.loadingFailed = true;
                    self.selectedWorkflow = null;
                    if (self.goJsDiagram) self.goJsDiagram.model = new go.GraphLinksModel([], []);
                });
        },
        selectWorkflow: function(workflow) {
            this.selectedWorkflow = workflow;
            this.$nextTick(() => {
                if (this.selectedWorkflow && this.selectedWorkflow.stages && this.selectedWorkflow.stages.length > 0) {
                    this.initGoJsDiagram(); 
                    this.updateGoJsModel();
                } else if (this.goJsDiagram) {
                    this.updateGoJsModel(); // This will clear the model
                }
            });
        },
        truncateDescription: function(text, length) {
            if (!text) return '';
            return text.length > length ? text.substring(0, length) + '...' : text;
        },
        retry: function() {
            this.load(); 
        },
        toggleEnabled: function(workflow) {
            var self = this;
            var action = workflow.isEnabled ? "Disable" : "Enable";
            var message = action + " Workflow";
            var confirmMessage = "Are you sure you want to " + action.toLowerCase() + " the workflow '" + workflow.title + "'?";
            if (!workflow.isEnabled && !self.items.some(w => w.isDefault)) {
                 confirmMessage += " This will also set it as the default workflow as no other default is set.";
            } else if (workflow.isEnabled && workflow.isDefault && self.items.filter(w => w.isEnabled && w.id !== workflow.id).length === 0) {
                piranha.notifications.push({ body: "Cannot disable the last enabled default workflow.", type: "warning", hide: true });
                return;
            }


            piranha.alert.confirmThis(message, confirmMessage, function () {
                fetch(piranha.baseUrl + "manager/api/workflow/" + workflow.id + "/toggle-enabled", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    }
                })
                .then(function (response) {
                    if (!response.ok) {
                        return response.json().then(function(err) { throw err; });
                    }
                    return response.json(); // Expecting the updated workflow
                })
                .then(function (updatedWorkflow) {
                    piranha.notifications.push({ body: "Workflow '" + updatedWorkflow.title + "' has been " + (updatedWorkflow.isEnabled ? "enabled" : "disabled") + ".", type: "success", hide: true });
                    // Reload all workflows to reflect potential changes in IsDefault status of other workflows
                    self.load().then(() => {
                        // Reselect the current workflow if it's still in the list
                        const reselected = self.items.find(item => item.id === updatedWorkflow.id);
                        if (reselected) {
                            self.selectWorkflow(reselected);
                        } else if (self.filteredItems.length > 0) {
                            self.selectWorkflow(self.filteredItems[0]);
                        } else {
                            self.selectedWorkflow = null;
                            self.updateGoJsModel(); // Clear diagram
                        }
                    });
                })
                .catch(function (error) {
                    console.error("Error toggling workflow enabled state:", error);
                    piranha.notifications.push({ body: error.body || "Failed to " + action.toLowerCase() + " workflow.", type: "danger", hide: true });
                });
            });
        },
        createWorkflow: function () {
            var self = this;

            if (!self.newWorkflowTitle.trim()) {
                piranha.notifications.push({
                    body: "Title is required for the new workflow.",
                    type: "danger",
                    hide: true
                });
                return;
            }

            var model = {
                title: self.newWorkflowTitle,
                description: self.newWorkflowDescription
            };

            fetch(piranha.baseUrl + "manager/api/workflow/create-standard", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    // Include AntiForgeryToken if your setup requires it for POSTs
                    // "RequestVerificationToken": анти forgery token value
                },
                body: JSON.stringify(model)
            })
            .then(function (response) {
                if (!response.ok) {
                    return response.json().then(function(err) { throw err; });
                }
                return response.json();
            })
            .then(function (newWorkflow) {
                piranha.notifications.push({
                    body: "Workflow '" + newWorkflow.title + "' created successfully.",
                    type: "success",
                    hide: true
                });
                // Reset form fields
                self.newWorkflowTitle = "";
                self.newWorkflowDescription = "";

                // Hide modal (jQuery needed if Bootstrap's JS is used for modals)
                if (typeof $ !== 'undefined' && $('#workflowModal').modal) {
                    $('#workflowModal').modal('hide');
                }


                // Refresh the list and select the new workflow
                self.load().then(function() {
                    // Find and select the newly created workflow
                    var created = self.items.find(function(item) { return item.id === newWorkflow.id; });
                    if (created) {
                        self.selectWorkflow(created);
                    }
                });
            })
            .catch(function (error) {
                console.error("Error creating workflow:", error);
                piranha.notifications.push({
                    body: error.body || "Failed to create workflow. Please try again.",
                    type: "danger",
                    hide: true
                });
            });
        },
        remove: function(id) {
            var self = this;
            piranha.alert.confirmThis("Delete Workflow", "Are you sure you want to delete this workflow?", function() {
                fetch(piranha.baseUrl + "manager/api/workflow/delete/" + id, { method: "DELETE" })
                    .then(function(response) { return response.json(); })
                    .then(function(result) {
                        var initialLength = self.items.length;
                        self.items = self.items.filter(item => item.id !== id);
                        
                        if (self.selectedWorkflow && self.selectedWorkflow.id === id) {
                            self.selectedWorkflow = self.filteredItems.length > 0 ? self.filteredItems[0] : null;
                        } else if (initialLength > self.items.length && self.filteredItems.length > 0 && !self.selectedWorkflow) {
                            self.selectedWorkflow = self.filteredItems[0];
                        } else if (self.filteredItems.length === 0) {
                            self.selectedWorkflow = null;
                        }
                        
                        self.$nextTick(() => {
                            if (self.selectedWorkflow && self.selectedWorkflow.stages && self.selectedWorkflow.stages.length > 0) {
                                self.initGoJsDiagram(); 
                                self.updateGoJsModel();
                            } else if (self.goJsDiagram) {
                                self.updateGoJsModel(); // Corrected typo from updateGoJsDiagramModel
                            }
                        });
                        
                        piranha.notifications.push({ type: "success", body: "The workflow has been deleted" });
                    })
                    .catch(function(error) {
                        console.error("Error deleting workflow:", error);
                        piranha.notifications.push({ type: "danger", body: "Something went wrong deleting the workflow" });
                    });
            });
        }
    },
    watch: {
        // It might be beneficial to watch selectedWorkflow directly if its internal properties (stages, relations)
        // could change without selectWorkflow being called.
        // For now, assuming selectWorkflow and load cover model updates.
        // 'selectedWorkflow.stages': { // Example of a deep watcher if needed
        //    handler: 'updateGoJsModel',
        //    deep: true
        // }
    },
    mounted: function() {
        if (piranha.permissions && typeof piranha.permissions.load === 'function') {
            piranha.permissions.load(() => {
                this.load();
            });
        } else {
            this.load(); 
        }
        // Add resize listener if you want the diagram to adapt to window resize
        // window.addEventListener('resize', this.handleResize);
    },
    beforeDestroy: function() {
        if (this.goJsDiagram) {
            this.goJsDiagram.div = null; 
            this.goJsDiagram = null;
        }
        // window.removeEventListener('resize', this.handleResize);
    }
});
