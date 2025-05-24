/*global
    piranha, Vue, go
*/

piranha.workflow = new Vue({
    el: "#workflow",
    data: {
        items: [],
        loading: true,
        loadingFailed: false,
        selectedWorkflow: null,
        searchTerm: "",
        goJsDiagram: null // Holds the GoJS Diagram instance
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
                console.warn("GoJS init: goJsDiagramDiv not found in refs.");
                return;
            }

            if (this.goJsDiagram) { 
                return; // Diagram already initialized
            }

            // Ensure GoJS is loaded
            if (typeof go === 'undefined') {
                console.error("GoJS library is not loaded.");
                return;
            }

            // If CSS height isn't applied in time, force it.
            if (this.$refs.goJsDiagramDiv.offsetHeight < 10) { 
                 console.warn("GoJS init (pre-create): goJsDiagramDiv has no significant height. Forcing height.", this.$refs.goJsDiagramDiv.offsetHeight);
                 this.$refs.goJsDiagramDiv.style.height = "400px";
                 // Optional: force browser to recalculate layout.
                 // void this.$refs.goJsDiagramDiv.offsetHeight; 
            }

            console.log("GoJS init: Initializing new Diagram instance. Div height now:", this.$refs.goJsDiagramDiv.offsetHeight);
            const $ = go.GraphObject.make; 

            this.goJsDiagram = $(go.Diagram, this.$refs.goJsDiagramDiv, {
                initialContentAlignment: go.Spot.Center,
                layout: $(go.LayeredDigraphLayout, { 
                    direction: 90, 
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
                $(go.Node, "Auto",
                    { 
                        locationSpot: go.Spot.Center,
                        fromSpot: go.Spot.BottomCenter, 
                        toSpot: go.Spot.TopCenter,     
                        selectionObjectName: "PANEL",
                        toolTip: $(go.Adornment, "Auto",
                                   $(go.TextBlock, { margin: 4 }, new go.Binding("text", "description"))
                                 )
                    },
                    new go.Binding("layerName", "isPublished", function(is) { return is ? "Foreground" : ""; }),
                    $(go.Shape, "RoundedRectangle",
                        { fill: "white", stroke: "#BBB", strokeWidth: 1, portId: "" },
                        new go.Binding("fill", "isPublished", function(is) { return is ? "#E6FFED" : "#FFFFFF"; }),
                        new go.Binding("stroke", "isPublished", function(is) { return is ? "#4CAF50" : "#BBB"; }),
                        new go.Binding("strokeWidth", "isPublished", function(is) { return is ? 2 : 1; })
                    ),
                    $(go.Panel, "Vertical", { margin: 10, defaultAlignment: go.Spot.Left },
                        $(go.TextBlock,
                            { font: "bold 10pt sans-serif", stroke: "#333", margin: new go.Margin(0, 0, 4, 0) },
                            new go.Binding("text", "title")
                        ),
                        $(go.TextBlock,
                            { font: "9pt sans-serif", stroke: "#555", wrap: go.TextBlock.WrapDesiredSize, width: 130 },
                            new go.Binding("text", "description", function(d) { return piranha.workflow.truncateDescription(d, 50); })
                        )
                    )
                );

            // Define the Link template
            this.goJsDiagram.linkTemplate =
                $(go.Link,
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
                    $(go.Shape, { strokeWidth: 2, stroke: "#555" }),
                    $(go.Shape, { toArrow: "Standard", fill: "#555", stroke: null, scale: 1.5 })
                );
            
        },
        updateGoJsModel: function() {
            if (!this.goJsDiagram) {
                // It's possible this gets called before initGoJsDiagram if the $nextTick order is tricky
                // or if initGoJsDiagram returned early.
                // Attempt to initialize if the div exists and conditions are met.
                if (this.selectedWorkflow && this.selectedWorkflow.stages && this.selectedWorkflow.stages.length > 0 && this.$refs.goJsDiagramDiv) {
                    console.warn("GoJS updateGoJsModel: Diagram not initialized, attempting init now.");
                    this.initGoJsDiagram(); // Try to init
                    if (!this.goJsDiagram) { // If init still failed (e.g. GoJS lib not loaded)
                        console.error("GoJS updateGoJsModel: Failed to initialize diagram on retry.");
                        return;
                    }
                } else {
                    console.warn("GoJS updateGoJsModel: Diagram not initialized and conditions for init not met.");
                    return;
                }
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

            console.log("GoJS updateGoJsModel: Nodes:", nodeDataArray.length, "Links:", linkDataArray.length);

            this.goJsDiagram.model = new go.GraphLinksModel(nodeDataArray, linkDataArray);
            
            // Crucial: Tell GoJS to re-evaluate its div and layout.
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

// Register utils if needed
if (!piranha.utils) {
    piranha.utils = {
        generateId: function () {
            return Math.random().toString(36).substring(2, 15) +
                Math.random().toString(36).substring(2, 15);
        }
    };
}
