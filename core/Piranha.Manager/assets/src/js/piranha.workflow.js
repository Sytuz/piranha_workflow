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
            const $go = go.GraphObject.make;

            // --- Begin: Use the same node template as workflowedit.js for color bar ---
            this.goJsDiagram = $go(go.Diagram, this.$refs.goJsDiagramDiv, {
                initialContentAlignment: go.Spot.Center,
                initialScale: 0.95,
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

            this.goJsDiagram.nodeTemplate =
                $go(go.Node, "Auto",
                    $go(go.Panel, "Table",
                        { defaultAlignment: go.Spot.Left, margin: 0 },
                        // Left color bar with rounded top-left and bottom-left corners
                        $go(go.Shape, "Rectangle",
                            {
                                row: 0, column: 0,
                                width: 12,
                                stretch: go.GraphObject.Vertical,
                                fill: "#cccccc",
                                stroke: null,
                                minSize: new go.Size(12, 40),
                                maxSize: new go.Size(12, NaN),
                                margin: 0,
                                parameter1: 0,
                                geometryString: "F M12,0 Q0,0 0,8 V72 Q0,80 12,80 H12 V0 Z"
                            },
                            new go.Binding("fill", "color")
                        ),
                        // Main node shape and content: Rectangle with rounded top-right and bottom-right corners
                        $go(go.Panel, "Auto",
                            { row: 0, column: 1 },
                            $go(go.Shape, "Rectangle",
                                {
                                    fill: "white",
                                    stroke: "#BBB",
                                    strokeWidth: 1,
                                    minSize: new go.Size(120, 40),
                                    parameter1: 0,
                                    geometryString: "F M0,0 H108 Q120,0 120,8 V72 Q120,80 108,80 H0 Q0,80 0,72 V8 Q0,0 0,0 Z"
                                }
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
                        )
                    ),
                    {
                        toolTip: $go(go.Adornment, "Auto",
                            $go(go.TextBlock, { margin: 4 }, new go.Binding("text", "description"))
                        )
                    }
                );
            // --- End: Use the same node template as workflowedit.js for color bar ---

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

            // --- Add color property to nodeDataArray ---
            const nodeDataArray = (this.selectedWorkflow.stages || []).map(stage => ({
                key: stage.id,
                title: stage.title,
                description: stage.description,
                isPublished: stage.isPublished,
                color: stage.color || "#cccccc"
            }));
            // --- End color property ---

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
        toggleEnabled: function(workflowToEnable) {
            var self = this;
            // This function now only ENABLES a workflow. Disabling happens automatically to others.
            if (workflowToEnable.isEnabled) {
                piranha.notifications.push({ body: "Workflow '" + workflowToEnable.title + "' is already the active workflow.", type: "info", hide: true });
                return;
            }

            var message = "Enable Workflow";
            var confirmMessage = "Are you sure you want to enable the workflow '" + workflowToEnable.title + "'? This will disable any other currently active workflow.";

            piranha.alert.open({
                title: message,
                body: confirmMessage,
                confirmText: "Enable",
                confirmCss: "btn-success",
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/api/workflow/" + workflowToEnable.id + "/toggle-enabled", {
                        method: "POST", // Assuming the backend is adjusted for this to mean "make this one enabled"
                        headers: {
                            "Content-Type": "application/json",
                        }
                    })
                    .then(function (response) {
                        if (!response.ok) {
                            // Try to parse error response as JSON
                            return response.json().then(function(errData) { 
                                throw new Error(errData.message || "Server error"); 
                            }).catch(function() {
                                // If not JSON, or no message, throw generic error
                                throw new Error("Failed to enable workflow. Status: " + response.status);
                            });
                        }
                        // No specific JSON body is expected on success from ToggleEnabledAsync as it just saves.
                        // The success is confirmed by reloading the list.
                        return response.text(); // Or response.json() if backend sends back the updated workflow or all workflows
                    })
                    .then(function () {
                        piranha.notifications.push({ body: "Workflow '" + workflowToEnable.title + "' has been enabled.", type: "success", hide: true });
                        // Reload all workflows to reflect the change in active status.
                        self.load().then(() => {
                            // Reselect the (now) enabled workflow.
                            const reselected = self.items.find(item => item.id === workflowToEnable.id);
                            if (reselected) {
                                self.selectWorkflow(reselected);
                            } else if (self.filteredItems.length > 0) {
                                // Fallback if somehow the enabled one isn't found (should not happen)
                                self.selectWorkflow(self.filteredItems[0]);
                            } else {
                                self.selectedWorkflow = null;
                                if (self.goJsDiagram) self.updateGoJsModel(); // Clear diagram
                            }
                        });
                    })
                    .catch(function (error) {
                        console.error("Error enabling workflow:", error);
                        piranha.notifications.push({ body: error.message || "Failed to enable workflow.", type: "danger", hide: true });
                    });
                }
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
            var workflowToRemove = self.items.find(item => item.id === id);
            if (!workflowToRemove) {
                console.error("Workflow to remove not found:", id);
                piranha.notifications.push({ body: "Could not find the workflow to delete.", type: "danger", hide: true });
                return;
            }

            // Prevent deleting the active workflow from the UI side as well, though backend enforces it.
            if (workflowToRemove.isEnabled) {
                piranha.notifications.push({ body: "Cannot delete the active workflow. Please enable another workflow first.", type: "warning", hide: true });
                return;
            }

            piranha.alert.open({
                title: "Delete Workflow",
                body: "Are you sure you want to delete the workflow '" + workflowToRemove.title + "'?",
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete || "Delete", // Fallback if texts.delete is not defined
                onConfirm: function() {
                    fetch(piranha.baseUrl + "manager/api/workflow/" + id, { method: "DELETE" })
                        .then(function(response) {
                            if (!response.ok) {
                                return response.text().then(function(text) {
                                    let finalDetailedMessage = text.trim(); // Default to the full trimmed server response text
                                    try {
                                        const parsedJson = JSON.parse(text.trim()); // Attempt to parse the trimmed text
                                        // If parsing is successful and a non-empty 'message' string exists, use it
                                        if (parsedJson && typeof parsedJson.message === 'string' && parsedJson.message.trim() !== '') {
                                            finalDetailedMessage = parsedJson.message.trim();
                                        }
                                        // If it's valid JSON but not the expected {message: ''} structure,
                                        // finalDetailedMessage will remain the full trimmed JSON string, which is acceptable.
                                    } catch (jsonParseError) {
                                        // Not JSON or malformed JSON. finalDetailedMessage is already the trimmed raw text.
                                        // Log the actual parsing error for diagnostics, but don't let it pollute the user-facing message.
                                        console.warn(`Error response from server (status ${response.status}) was not valid JSON or 'message' field was not found/string. Raw text: "${text}". Parse error: ${jsonParseError.message}`);
                                    }
                                    // Throw an error with a consistent prefix and the determined message
                                    throw new Error("Workflow deletion failed: " + finalDetailedMessage);
                                });
                            }
                            // For successful responses (like 204 NoContent), there might be no body or a non-JSON body.
                            // If a body is expected for success, it should be handled here.
                            // For DELETE leading to 204, just returning the response or nothing is fine.
                            return response; 
                        })
                        .then(function(response) { // Note: 'response' here might be undefined if the previous .then threw an error.
                                                  // Or it's the original response object for success cases.
                            // Only proceed with success notifications if the operation didn't throw an error.
                            // The actual check for response.ok was done, and errors were thrown.
                            // If we reach here without an error, it implies a successful HTTP status.
                            piranha.notifications.push({ type: "success", body: "The workflow has been deleted" });
                            // Update local state
                            self.items = self.items.filter(item => item.id !== id);
                            if (self.selectedWorkflow && self.selectedWorkflow.id === id) {
                                self.selectedWorkflow = self.filteredItems.length > 0 ? self.filteredItems[0] : null;
                            } else if (self.filteredItems.length === 0) {
                                self.selectedWorkflow = null;
                            }

                            // Refresh the diagram model if it was displayed
                            if (self.goJsDiagram) {
                                self.updateGoJsModel(); 
                            }
                        })
                        .catch(function(error) {
                            console.error("Error deleting workflow:", error);
                            piranha.notifications.push({ type: "danger", body: error.body || error.message || "Something went wrong deleting the workflow" });
                        });
                }
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
