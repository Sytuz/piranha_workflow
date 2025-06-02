piranha.workflowdashboard = new function () {
    "use strict";

    var self = this;

    // Vue app instance
    self.app = null;

    // Initialize the workflow dashboard
    self.init = function () {
        if (document.getElementById("workflow-dashboard")) {
            self.app = new Vue({
                el: "#workflow-dashboard",
                data: {
                    loading: true,
                    error: null,
                    activeTab: 'overview',
                    analyticsTimeRange: 30,
                    overview: null,
                    analytics: null,
                    changeHistory: null,
                    loadingChanges: false,
                    changeFilters: {
                        contentType: '',
                        changeType: '',
                        user: '',
                        page: 1,
                        pageSize: 20
                    },
                    changeHistoryDebounceTimeout: null,
                    // Change request details
                    selectedChangeRequestId: null,
                    changeRequestDetails: null,
                    loadingDetails: false,
                    detailsError: null,
                    // Action handling
                    pendingAction: null,
                    actionData: {
                        comments: '',
                        reason: ''
                    },
                    executingAction: false,
                    actionError: null
                },
                computed: {
                    maxStageCount: function () {
                        if (!this.overview || !this.overview.stageDistribution) return 0;
                        return Math.max(...this.overview.stageDistribution.map(s => s.count), 1);
                    }
                },
                mounted: function () {
                    this.loadOverview();
                },
                methods: {
                    // Tab management
                    setActiveTab: function (tab) {
                        this.activeTab = tab;
                        if (tab === 'analytics' && !this.analytics) {
                            this.loadAnalytics();
                        } else if (tab === 'changes' && !this.changeHistory) {
                            this.loadChangeHistory();
                        }
                    },

                    // Data loading methods
                    loadOverview: function () {
                        var self = this;
                        self.loading = true;
                        self.error = null;

                        fetch(piranha.baseUrl + "manager/api/workflow-dashboard/overview", {
                            method: "GET",
                            headers: {
                                "Content-Type": "application/json"
                            }
                        })
                        .then(function (response) {
                            if (!response.ok) {
                                throw new Error("Failed to load overview data");
                            }
                            return response.json();
                        })
                        .then(function (data) {
                            self.overview = data;
                            self.loading = false;
                        })
                        .catch(function (error) {
                            console.error("Error loading overview:", error);
                            self.error = error.message || "Failed to load dashboard overview";
                            self.loading = false;
                        });
                    },

                    loadAnalytics: function () {
                        var self = this;
                        self.loading = true;
                        self.error = null;

                        fetch(piranha.baseUrl + "manager/api/workflow-dashboard/analytics?days=" + self.analyticsTimeRange, {
                            method: "GET",
                            headers: {
                                "Content-Type": "application/json"
                            }
                        })
                        .then(function (response) {
                            if (!response.ok) {
                                throw new Error("Failed to load analytics data");
                            }
                            return response.json();
                        })
                        .then(function (data) {
                            self.analytics = data;
                            self.loading = false;
                            // Initialize charts after data is loaded
                            self.$nextTick(function () {
                                self.initializeCharts();
                            });
                        })
                        .catch(function (error) {
                            console.error("Error loading analytics:", error);
                            self.error = error.message || "Failed to load analytics";
                            self.loading = false;
                        });
                    },

                    loadChangeHistory: function () {
                        var self = this;
                        self.loadingChanges = true;

                        var params = new URLSearchParams();
                        if (self.changeFilters.contentType) params.append('contentType', self.changeFilters.contentType);
                        if (self.changeFilters.changeType) params.append('changeType', self.changeFilters.changeType);
                        if (self.changeFilters.user) params.append('user', self.changeFilters.user);
                        params.append('page', self.changeFilters.page);
                        params.append('pageSize', self.changeFilters.pageSize);

                        fetch(piranha.baseUrl + "manager/api/workflow-dashboard/changes?" + params.toString(), {
                            method: "GET",
                            headers: {
                                "Content-Type": "application/json"
                            }
                        })
                        .then(function (response) {
                            if (!response.ok) {
                                throw new Error("Failed to load change history");
                            }
                            return response.json();
                        })
                        .then(function (data) {
                            self.changeHistory = data;
                            self.loadingChanges = false;
                        })
                        .catch(function (error) {
                            console.error("Error loading change history:", error);
                            self.error = error.message || "Failed to load change history";
                            self.loadingChanges = false;
                        });
                    },

                    // Analytics time range
                    setAnalyticsTimeRange: function (days) {
                        this.analyticsTimeRange = days;
                        this.loadAnalytics();
                    },

                    // Change history pagination
                    changeHistoryPage: function (page) {
                        if (page >= 1 && page <= this.changeHistory.totalPages) {
                            this.changeFilters.page = page;
                            this.loadChangeHistory();
                        }
                    },

                    // Change history filters
                    clearChangeFilters: function () {
                        this.changeFilters = {
                            contentType: '',
                            changeType: '',
                            user: '',
                            page: 1,
                            pageSize: 20
                        };
                        this.loadChangeHistory();
                    },

                    debounceLoadChangeHistory: function () {
                        var self = this;
                        if (self.changeHistoryDebounceTimeout) {
                            clearTimeout(self.changeHistoryDebounceTimeout);
                        }
                        self.changeHistoryDebounceTimeout = setTimeout(function () {
                            self.changeFilters.page = 1; // Reset to first page
                            self.loadChangeHistory();
                        }, 500);
                    },

                    // Data refresh
                    refreshData: function () {
                        if (this.activeTab === 'overview') {
                            this.loadOverview();
                        } else if (this.activeTab === 'analytics') {
                            this.loadAnalytics();
                        } else if (this.activeTab === 'changes') {
                            this.loadChangeHistory();
                        }
                    },

                    // Change request details methods
                    viewChangeRequestDetails: function (changeRequestId) {
                        this.selectedChangeRequestId = changeRequestId;
                        this.loadChangeRequestDetails(changeRequestId);
                        $('#changeRequestDetailsModal').modal('show');
                    },

                    loadChangeRequestDetails: function (changeRequestId) {
                        var self = this;
                        self.loadingDetails = true;
                        self.detailsError = null;
                        self.changeRequestDetails = null;

                        fetch(piranha.baseUrl + "manager/api/changerequest/" + changeRequestId + "/details", {
                            method: "GET",
                            headers: {
                                "Content-Type": "application/json"
                            }
                        })
                        .then(function (response) {
                            if (!response.ok) {
                                throw new Error("Failed to load change request details");
                            }
                            return response.json();
                        })
                        .then(function (data) {
                            self.changeRequestDetails = data;
                            self.loadingDetails = false;
                        })
                        .catch(function (error) {
                            console.error("Error loading change request details:", error);
                            self.detailsError = error.message || "Failed to load change request details";
                            self.loadingDetails = false;
                        });
                    },

                    executeAction: function (action, changeRequestId) {
                        this.pendingAction = action;
                        this.selectedChangeRequestId = changeRequestId;
                        this.actionData = {
                            comments: '',
                            reason: ''
                        };
                        this.actionError = null;
                        $('#actionConfirmationModal').modal('show');
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
                        var payload = {
                            userId: piranha.userId || "00000000-0000-0000-0000-000000000000"
                        };
                        if (self.pendingAction.type === 'approve') {
                            endpoint = "approve";
                            if (self.actionData.comments) payload.comments = self.actionData.comments;
                        } else if (self.pendingAction.type === 'reject') {
                            endpoint = "reject";
                            payload.reason = self.actionData.reason;
                        } else if (self.pendingAction.type === 'edit') {
                            endpoint = "edit";
                        }
                        fetch(piranha.baseUrl + "manager/api/changerequest/" + self.selectedChangeRequestId + "/" + endpoint, {
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
                            $('#changeRequestDetailsModal').modal('hide');
                            self.refreshData();
                        })
                        .catch(function (error) {
                            self.executingAction = false;
                            self.actionError = error.message || "Failed to execute action.";
                        });
                    },

                    // Helper methods for UI
                    getChangeStatusBadgeClass: function (status) {
                        switch (status) {
                            case 0: // Draft
                                return 'badge-secondary';
                            case 1: // Submitted
                                return 'badge-primary';
                            case 2: // InReview
                                return 'badge-info';
                            case 3: // Approved
                                return 'badge-success';
                            case 4: // Rejected
                                return 'badge-danger';
                            case 5: // Published
                                return 'badge-success';
                            default:
                                return 'badge-secondary';
                        }
                    },

                    getChangeStatusText: function (status) {
                        switch (status) {
                            case 0:
                                return 'Draft';
                            case 1:
                                return 'Submitted';
                            case 2:
                                return 'In Review';
                            case 3:
                                return 'Approved';
                            case 4:
                                return 'Rejected';
                            case 5:
                                return 'Published';
                            default:
                                return 'Unknown';
                        }
                    },

                    getActionButtonClass: function (actionType) {
                        switch (actionType) {
                            case 'approve':
                                return 'btn-success';
                            case 'reject':
                                return 'btn-danger';
                            case 'edit':
                                return 'btn-primary';
                            case 'move':
                                return 'btn-warning';
                            default:
                                return 'btn-secondary';
                        }
                    },

                    getStagePercentage: function (count) {
                        if (this.maxStageCount === 0) return 0;
                        return (count / this.maxStageCount) * 100;
                    },

                    getBottleneckCount: function () {
                        if (!this.analytics || !this.analytics.bottlenecks) return 0;
                        return this.analytics.bottlenecks.filter(b => b.bottleneckSeverity >= 2).length;
                    },

                    getCompletionRateBadgeClass: function (rate) {
                        if (rate >= 90) return 'badge-success';
                        if (rate >= 70) return 'badge-warning';
                        return 'badge-danger';
                    },

                    getContentTypeBadgeClass: function (type) {
                        return type === 'Page' ? 'badge-primary' : 'badge-info';
                    },

                    getChangeTypeBadgeClass: function (type) {
                        switch (type.toLowerCase()) {
                            case 'created': return 'badge-success';
                            case 'updated': return 'badge-info';
                            case 'approved': return 'badge-success';
                            case 'rejected': return 'badge-danger';
                            default: return 'badge-secondary';
                        }
                    },

                    getPageNumbers: function () {
                        if (!this.changeHistory) return [];
                        
                        var total = this.changeHistory.totalPages;
                        var current = this.changeHistory.currentPage;
                        var pages = [];
                        
                        // Show max 5 page numbers
                        var start = Math.max(1, current - 2);
                        var end = Math.min(total, start + 4);
                        
                        for (var i = start; i <= end; i++) {
                            pages.push(i);
                        }
                        
                        return pages;
                    },

                    formatDateTime: function (dateString) {
                        if (!dateString) return '';
                        var date = new Date(dateString);
                        return date.toLocaleString();
                    },

                    // Chart initialization
                    initializeCharts: function () {
                        if (typeof Chart === 'undefined' || !this.analytics || !this.analytics.dailyStats) {
                            return;
                        }

                        var ctx = document.getElementById('dailyActivityChart');
                        if (!ctx) return;

                        // Clear existing chart if any
                        if (this.dailyActivityChart) {
                            this.dailyActivityChart.destroy();
                        }

                        var labels = this.analytics.dailyStats.map(function (stat) {
                            return new Date(stat.date).toLocaleDateString();
                        });

                        var createdData = this.analytics.dailyStats.map(function (stat) {
                            return stat.itemsCreated;
                        });

                        var approvedData = this.analytics.dailyStats.map(function (stat) {
                            return stat.itemsApproved;
                        });

                        var completedData = this.analytics.dailyStats.map(function (stat) {
                            return stat.itemsCompleted;
                        });

                        this.dailyActivityChart = new Chart(ctx, {
                            type: 'line',
                            data: {
                                labels: labels,
                                datasets: [
                                    {
                                        label: 'Created',
                                        data: createdData,
                                        borderColor: 'rgb(54, 162, 235)',
                                        backgroundColor: 'rgba(54, 162, 235, 0.1)',
                                        tension: 0.1
                                    },
                                    {
                                        label: 'Approved',
                                        data: approvedData,
                                        borderColor: 'rgb(75, 192, 192)',
                                        backgroundColor: 'rgba(75, 192, 192, 0.1)',
                                        tension: 0.1
                                    },
                                    {
                                        label: 'Completed',
                                        data: completedData,
                                        borderColor: 'rgb(255, 99, 132)',
                                        backgroundColor: 'rgba(255, 99, 132, 0.1)',
                                        tension: 0.1
                                    }
                                ]
                            },
                            options: {
                                responsive: true,
                                maintainAspectRatio: false,
                                scales: {
                                    y: {
                                        beginAtZero: true,
                                        ticks: {
                                            stepSize: 1
                                        }
                                    }
                                },
                                plugins: {
                                    legend: {
                                        position: 'top',
                                    },
                                    title: {
                                        display: true,
                                        text: 'Daily Workflow Activity'
                                    }
                                }
                            }
                        });
                    }
                }
            });
        }
    };
};

// Initialize the workflow dashboard when the page loads
$(document).ready(function () {
    piranha.workflowdashboard.init();
});
