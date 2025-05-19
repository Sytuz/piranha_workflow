/*global
    piranha
*/

piranha.workflow = {
    load: function (callback) {
        fetch(piranha.baseUrl + "manager/api/workflow/list")
            .then(function (response) { return response.json(); })
            .then(function (result) {
                if (callback)
                    callback(result);
            })
            .catch(function (error) { console.log("error:", error ); });
    }
};

$(document).ready(function () {
    piranha.workflow.load();
});
