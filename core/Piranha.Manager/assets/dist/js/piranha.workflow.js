/*global
    piranha
*/

piranha.workflow = new Vue({
    el: "#workflow",
    data: {
        loading: true,
        loadingFailed: false,
        items: [],
        currentWorkflowId: null
    },
    methods: {
        load: function () {
            var self = this;
            
            self.loading = true;
            self.loadingFailed = false;
            
            fetch(piranha.baseUrl + "manager/api/workflow/list")
                .then(function (response) { 
                    if (!response.ok) {
                        throw new Error("Server returned status: " + response.status);
                    }
                    return response.json(); 
                })
                .then(function (result) {
                    self.items = result;
                    self.loading = false;
                })
                .catch(function (error) { 
                    console.log("Error loading workflows:", error);
                    self.loading = false;
                    self.loadingFailed = true;
                    
                    piranha.notifications.push({
                        body: "Failed to load workflows. Please try again or contact your administrator.",
                        type: "danger"
                    });
                });
        },
        retry: function() {
            this.load();
        },
        remove: function (id) {
            this.currentWorkflowId = id;

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: "Are you sure you want to delete this workflow?",
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: this.removeConfirm
            });
        },
        removeConfirm: function () {
            var self = this;
            
            fetch(piranha.baseUrl + "manager/api/workflow/" + this.currentWorkflowId, {
                method: "DELETE"
            })
            .then(function (response) {
                if (response.ok) {
                    // Refresh the list
                    self.load();

                    // Clear the current id
                    self.currentWorkflowId = null;

                    // Show notification
                    piranha.notifications.push({
                        body: "The workflow was successfully deleted",
                        type: "success"
                    });
                } else {
                    // Show error
                    return response.json().then(function(err) {
                        throw new Error(err.body || "An error occurred while deleting the workflow");
                    });
                }
            })
            .catch(function (error) {
                console.log("Error deleting workflow:", error);
                piranha.notifications.push({
                    body: error.message || "An error occurred while deleting the workflow",
                    type: "danger"
                });
            });
        }
    }
});

$(document).ready(function () {
    piranha.workflow.load();
});
