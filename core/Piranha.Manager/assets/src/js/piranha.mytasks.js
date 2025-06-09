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
                    actionError: null
                },
                computed: {
                    filteredItems: function () {
                        var self = this;
                        return this.items.filter(function (item) {
                            if (self.state === "all") {
                                return true;
                            } else if (self.state === "pending") {
                                return item.status === "Pending";
                            } else if (self.state === "approved") {
                                return item.status === "Approved";
                            } else if (self.state === "rejected") {
                                return item.status === "Rejected";
                            }
                            return true;
                        });
                    }
                },
                mounted: function () {
                    this.loadTasks();
                },
                methods: {
                    // Load tasks from the API
                    loadTasks: function () {
                        var self = this;
                        self.loading = true;

                        fetch(piranha.baseUrl + "manager/api/mytasks", {
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
                                    editUrl: t.editUrl
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
                    },

                    // Set filter status
                    setStatus: function (status) {
                        this.state = status;
                    },

                    // Task action methods
                    approveTask: function (task) {
                        this.executeAction({
                            type: 'approve',
                            label: 'Approve Task'
                        }, task);
                    },

                    rejectTask: function (task) {
                        this.executeAction({
                            type: 'reject',
                            label: 'Reject Task'
                        }, task);
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

                    // Helper methods
                    formatDate: function (dateString) {
                        if (!dateString) return '';
                        var date = new Date(dateString);
                        return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
                    },

                    getStatusBadgeClass: function (status) {
                        switch (status) {
                            case 'Pending':
                                return 'badge-warning';
                            case 'Approved':
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
                    }
                },
                updated: function () {
                    // Additional setup can be done here if needed
                }
            });
        }
    };
};
