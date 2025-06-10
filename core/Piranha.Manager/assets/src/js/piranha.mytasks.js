/*global
    piranha
*/

piranha.mytasks = new function () {
    "use strict";

    var self = this;

    // Load function called from the page
    self.load = function () {
        self.init();
    };

    // Initialize the My Tasks page
    self.init = function () {
        if (document.getElementById("mytasks")) {
            new Vue({
                el: "#mytasks",
                data: {
                    loading: true,
                    items: [],
                    state: "all",
                    // Action handling
                    pendingAction: null,
                    selectedTask: null,
                    actionData: {
                        comments: '',
                        reason: ''
                    },
                    executingAction: false,
                    actionError: null,
                    // New data properties for approval modal
                    approvalWorkflowData: null,
                    approvalAccessibleStages: [],
                    selectedApproveTargetStage: null,
                    approveComments: '',
                    approveTargetStageError: null,
                    submittingApproveChangeRequest: false,
                    // New data properties for rejection modal
                    rejectReason: '',
                    rejectReasonError: null,
                    submittingRejectChangeRequest: false
                },                computed: {
                    filteredItems: function () {
                        // Since we're now filtering on the server side for approved/rejected,
                        // we just return all items for client-side filtering
                        var self = this;
                        return this.items.filter(function (item) {
                            if (self.state === "all") {
                                return true;
                            } else if (self.state === "pending") {
                                return item.status === "Pending" || item.status === "Draft" || item.status === "InReview";
                            } else if (self.state === "approved") {
                                // For approved/rejected, server already filtered, so show all
                                return true;
                            } else if (self.state === "rejected") {
                                // For approved/rejected, server already filtered, so show all
                                return true;
                            }
                            return true;
                        });
                    }
                },
                mounted: function () {
                    this.loadTasks();
                },                methods: {
                    // Load tasks from the API
                    loadTasks: function () {
                        var self = this;
                        self.loading = true;

                        // Construct URL with filter parameter
                        var url = piranha.baseUrl + "manager/api/mytasks";
                        if (self.state !== "all") {
                            url += "?filter=" + encodeURIComponent(self.state);
                        }

                        fetch(url, {
                            method: "GET",
                            headers: {
                                "Content-Type": "application/json"
                            }
                        })
                        .then(function (response) {
                            if (!response.ok) {
                                throw new Error("Failed to load my tasks data");
                            }
                            return response.json();
                        })
                        .then(function (data) {

                            // Ensure tasks is always an array
                            var rawTasks = data.tasks;
                            if (!Array.isArray(rawTasks)) {
                                rawTasks = Object.values(rawTasks || {});
                            }

                            var tasks = rawTasks.map(function (t, idx) {
                                return {
                                    id: t.id,
                                    contentTitle: t.contentTitle,
                                    contentType: t.contentType,
                                    workflowName: t.workflowName,
                                    currentStage: t.currentStage,
                                    status: t.status,
                                    created: t.timestamp,
                                    notes: t.notes,
                                    availableActions: t.availableActions || [],
                                    editUrl: t.editUrl,
                                    transitionPath: t.transitionPath,
                                    acceptanceTime: t.acceptanceTime
                                };
                            });

                            self.items = tasks;
                            self.loading = false;
                        })
                        .catch(function (error) {
                            console.error("Error loading my tasks:", error);
                            piranha.notifications.push({
                                type: 'error',
                                body: error.message || "Failed to load my tasks"
                            });
                            self.loading = false;
                        });
                    },                    // Set filter status and reload tasks
                    setStatus: function (status) {
                        this.state = status;
                        this.loadTasks(); // Reload tasks with new filter
                    },

                    // Helper methods for task actions
                    canEdit: function (task) {
                        return task.availableActions && task.availableActions.includes('edit');
                    },

                    canView: function (task) {
                        return task.availableActions && task.availableActions.includes('view');
                    },

                    canApprove: function (task) {
                        return task.availableActions && task.availableActions.includes('approve');
                    },

                    canReject: function (task) {
                        return task.availableActions && task.availableActions.includes('reject');
                    },

                    getEditButtonText: function (task) {
                        // Show "Edit" for draft status, "View" for non-draft status
                        return task.status === 'Draft' ? 'Edit' : 'View';
                    },

                    getEditButtonIcon: function (task) {
                        // Show edit icon for draft status, view icon for non-draft status
                        return task.status === 'Draft' ? 'fas fa-edit' : 'fas fa-eye';
                    },

                    // Task action methods
                    approveTask: function (task) {
                        var self = this;
                        fetch(piranha.baseUrl + "manager/api/changerequest/" + task.id + "/details", {
                            method: "GET",
                            headers: { "Content-Type": "application/json" }
                        })
                        .then(function (response) {
                            if (!response.ok) throw new Error("Failed to load workflow details");
                            return response.json();
                        })
                        .then(function (data) {
                            var workflow = data.workflow;
                            var currentStage = data.currentStage;
                            
                            var relations = workflow && workflow.relations ? workflow.relations : [];
                            var stages = workflow && workflow.stages ? workflow.stages : [];
                            
                            var nextStageIds = relations.filter(function(rel) {
                                return rel.sourceStageId === currentStage.id;
                            }).map(function(rel) { return rel.targetStageId; });
                            
                            var nextStages = stages.filter(function(stage) {
                                return nextStageIds.indexOf(stage.id) !== -1;
                            });
                            self.approvalWorkflowData = workflow;
                            self.approvalAccessibleStages = nextStages;
                            self.selectedApproveTargetStage = '';
                            self.selectedTask = task;
                            self.approveComments = '';
                            self.approveTargetStageError = null;
                            self.submittingApproveChangeRequest = false;
                            
                            // Show modal and initialize diagram
                            $('#approveChangeRequestModal').modal('show');
                            
                            // Initialize workflow diagram after modal is shown
                            $('#approveChangeRequestModal').on('shown.bs.modal', function () {
                                self.$nextTick(() => {
                                    self.initApprovalDiagram();
                                });
                            });
                        })
                        .catch(function (error) {
                            piranha.notifications.push({ type: 'error', body: error.message || "Failed to load workflow details" });
                        });
                    },

                    rejectTask: function (task) {
                        var self = this;
                        self.selectedTask = task;
                        self.rejectReason = '';
                        self.rejectReasonError = null;
                        self.submittingRejectChangeRequest = false;
                        $('#rejectChangeRequestModal').modal('show');
                    },                    
                    executeAction: function (action, task) {

                        // Reset all relevant state properties
                        this.pendingAction = null;
                        this.selectedTask = null;
                        this.actionData = {
                            comments: '',
                            reason: ''
                        };
                        this.actionError = null;
                        this.executingAction = false;

                        // Use $nextTick to allow Vue to process the reset, then set new state
                        var self = this;
                        this.$nextTick(function() {
                            self.pendingAction = action;
                            self.selectedTask = task;
                            $('#actionConfirmationModal').modal('show');
                        });
                    },

                    confirmAction: function () {
                        var self = this;

                        // Validate required fields
                        if (self.pendingAction.type === 'reject' && !self.actionData.reason.trim()) {
                            self.actionError = "Rejection reason is required.";
                            return;
                        }

                        self.executingAction = true;
                        self.actionError = null;

                        var endpoint = "";
                        var payload = {};

                        if (self.pendingAction.type === 'approve') {
                            endpoint = "approve";
                            if (self.actionData.comments) {
                                payload.comments = self.actionData.comments;
                            }
                        } else if (self.pendingAction.type === 'reject') {
                            endpoint = "reject";
                            payload.reason = self.actionData.reason;
                        }

                        fetch(piranha.baseUrl + "manager/api/mytasks/" + self.selectedTask.id + "/" + endpoint, {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/json",
                                ...piranha.utils.antiForgeryHeaders()
                            },
                            credentials: "same-origin",
                            body: JSON.stringify(payload)
                        })
                        .then(function (response) {
                            if (!response.ok) {
                                throw new Error("Failed to execute action");
                            }
                            return response.json();
                        })
                        .then(function (result) {
                            self.executingAction = false;
                            $('#actionConfirmationModal').modal('hide');

                            // Show success notification
                            piranha.notifications.push({
                                type: 'success',
                                body: 'Task ' + self.pendingAction.type + 'd successfully'
                            });

                            // Refresh the task list
                            self.loadTasks();
                        })
                        .catch(function (error) {
                            self.executingAction = false;
                            self.actionError = error.message || "Failed to execute action.";

                            piranha.notifications.push({
                                type: 'error',
                                body: self.actionError
                            });
                        });
                    },                    
                    cancelAction: function () {
                        this.pendingAction = null;
                        this.selectedTask = null;
                        this.actionData = {
                            comments: '',
                            reason: ''
                        };
                        this.actionError = null;
                        this.executingAction = false;
                        $('#actionConfirmationModal').modal('hide');
                    },

                    // Approve modal submit handler
                    submitApproveChangeRequest: function () {
                        var self = this;
                        if (!self.selectedApproveTargetStage) {
                            self.approveTargetStageError = "Please select a next stage.";
                            return;
                        }
                        self.submittingApproveChangeRequest = true;
                        var payload = {
                            comments: self.approveComments,
                            nextStageId: self.selectedApproveTargetStage
                        };
                        fetch(piranha.baseUrl + "manager/api/mytasks/" + self.selectedTask.id + "/approve", {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/json",
                                ...piranha.utils.antiForgeryHeaders()
                            },
                            credentials: "same-origin",
                            body: JSON.stringify(payload)
                        })
                        .then(function (response) {
                            if (!response.ok) throw new Error("Failed to approve task");
                            return response.json();
                        })
                        .then(function (result) {
                            self.submittingApproveChangeRequest = false;
                            $('#approveChangeRequestModal').modal('hide');
                            piranha.notifications.push({ type: 'success', body: 'Task approved successfully' });
                            self.loadTasks();
                        })
                        .catch(function (error) {
                            self.submittingApproveChangeRequest = false;
                            self.approveTargetStageError = error.message || "Failed to approve task.";
                        });
                    },

                    // Reject modal submit handler
                    submitRejectChangeRequest: function () {
                        var self = this;
                        if (!self.rejectReason || !self.rejectReason.trim()) {
                            self.rejectReasonError = "Rejection reason is required.";
                            return;
                        }
                        self.submittingRejectChangeRequest = true;
                        var payload = { reason: self.rejectReason };
                        fetch(piranha.baseUrl + "manager/api/mytasks/" + self.selectedTask.id + "/reject", {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/json",
                                ...piranha.utils.antiForgeryHeaders()
                            },
                            credentials: "same-origin",
                            body: JSON.stringify(payload)
                        })
                        .then(function (response) {
                            if (!response.ok) throw new Error("Failed to reject task");
                            return response.json();
                        })
                        .then(function (result) {
                            self.submittingRejectChangeRequest = false;
                            $('#rejectChangeRequestModal').modal('hide');
                            piranha.notifications.push({ type: 'success', body: 'Task rejected successfully' });
                            self.loadTasks();
                        })
                        .catch(function (error) {
                            self.submittingRejectChangeRequest = false;
                            self.rejectReasonError = error.message || "Failed to reject task.";
                        });
                    },

                    // Helper methods
                    formatDate: function (dateString) {
                        if (!dateString) return '';
                        var date = new Date(dateString);
                        return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
                    },

                    getStatusBadgeClass: function (status) {
                        switch (status) {
                            case 'InReview':
                                return 'badge-warning';
                            case 'Published':
                                return 'badge-success';
                            case 'Rejected':
                                return 'badge-danger';
                            case 'Draft':
                                return 'badge-secondary';
                            default:
                                return 'badge-secondary';
                        }
                    },

                    refreshTasks: function () {
                        this.loadTasks();
                    },

                    // Show transition path as tooltip or expandable row
                    getTransitionPath: function(task) {
                        if (task.transitionPath) {
                            // Replace stage IDs with arrows for now; ideally, resolve to stage names
                            return task.transitionPath.replace(/→/g, ' → ');
                        }
                        return 'No path';
                    },
                    getAcceptanceTime: function(task) {
                        if (task.acceptanceTime != null) {
                            return task.acceptanceTime.toFixed(2) + ' hrs';
                        }
                        return 'N/A';
                    },

                    // Initialize GoJS diagram for approval modal
                    initApprovalDiagram: function () {
                        if (!this.approvalWorkflowData || !this.approvalWorkflowData.stages || this.approvalWorkflowData.stages.length === 0) {
                            return;
                        }
                        
                        // Check if GoJS is available
                        if (typeof go === 'undefined') {
                            console.error("GoJS library not found for approval diagram");
                            return;
                        }
                        
                        var diagramDiv = document.getElementById("approveWorkflowDiagram");
                        if (!diagramDiv) {
                            console.error("Approval diagram container not found");
                            return;
                        }
                        
                        // Initialize GoJS diagram (same structure as PageEdit modal)
                        var $go = go.GraphObject.make;
                        var approvalDiagram = $go(go.Diagram, diagramDiv, {
                            layout: $go(go.LayeredDigraphLayout, {
                                direction: 0,
                                layerSpacing: 50,
                                columnSpacing: 30
                            }),
                            isReadOnly: true,
                            allowSelect: false,
                            allowCopy: false,
                            allowDelete: false,
                            allowMove: false,
                            hasHorizontalScrollbar: false,
                            hasVerticalScrollbar: false
                        });

                        // Define node template (same as PageEdit modal)
                        approvalDiagram.nodeTemplate =
                            $go(go.Node, "Auto",
                                $go(go.Panel, "Table",
                                    { defaultAlignment: go.Spot.Left, margin: 0 },
                                    // Left color bar
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
                                    // Main node content
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
                                                new go.Binding("text", "description", function(d) { 
                                                    return d && d.length > 50 ? d.substring(0, 47) + "..." : (d || ""); 
                                                })
                                            )
                                        )
                                    )
                                ),
                                { locationSpot: go.Spot.Center }
                            );
                        
                        // Define link template
                        approvalDiagram.linkTemplate =
                            $go(go.Link,
                                { routing: go.Link.AvoidsNodes, corner: 10 },
                                $go(go.Shape, { strokeWidth: 2, stroke: "#555" }),
                                $go(go.Shape, { toArrow: "Standard", fill: "#555", stroke: null })
                            );
                        
                        // Build node data
                        var nodeDataArray = this.approvalWorkflowData.stages.map(function(stage) {
                            return {
                                key: stage.id,
                                title: stage.title,
                                description: stage.description || "",
                                color: stage.color || "#cccccc"
                            };
                        });
                        
                        // Build link data
                        var linkDataArray = (this.approvalWorkflowData.relations || []).map(function(relation) {
                            return {
                                from: relation.sourceStageId,
                                to: relation.targetStageId
                            };
                        });
                        
                        // Set the model
                        approvalDiagram.model = new go.GraphLinksModel(nodeDataArray, linkDataArray);
                        approvalDiagram.requestUpdate();
                    },
                },
                updated: function () {
                    // Additional setup can be done here if needed
                }
            });
        }
    };
};
