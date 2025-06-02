/*global
    piranha, go, console
*/

// Prevent GoJS global redefinition warning
if (typeof window.go !== "undefined") {
    // Optionally log a warning, but do not redefine or reload GoJS
    console.warn("GoJS global object already defined. Skipping redefinition.");
}

piranha.workflowedit = new Vue({
    el: "#workflowedit",
    data: {
        loading: true,
        id: null,
        title: "",
        description: "",
        isDefault: false, // Indicates if this is the default workflow
        stages: [], // Stages will have { id, title, description }
        availableRoles: [],
        relations: [], // Stage relations: { sourceStageId, targetStageId }
        saveUrl: piranha.baseUrl + "manager/api/workflow/save",
        rolesUrl: piranha.baseUrl + "manager/api/workflow/roles",
        originalTitle: null,
         // GoJS diagram instance
            diagramInitialized: false,
            initAttempts: 0,
            maxInitAttempts: 5
            },
            methods: {
            bind: function (result) {
                //console.log("Bind function called with result:", result);
                
                this.id = result.id;
                this.title = result.title;
                this.originalTitle = result.title;
                this.description = result.description;
                this.isDefault = result.isDefault || false;
                
                //console.log("Processing stages from API:", result.stages);
                
                // Convert stages and roles from API format to UI format
                this.stages = (result.stages || []).map(function(stage) {
                //console.log("Processing stage:", stage);
                //console.log("Stage roles:", stage.roles);
                
                const roleIds = (stage.roles || []).map(function(role) {
                    //console.log("Processing role:", role);
                    const roleId = role.roleId || role.RoleId;
                    //console.log("Extracted roleId:", roleId);
                    return String(roleId).toUpperCase();
                }) || [];
                
                //console.log("Final roleIds for stage", stage.id, ":", roleIds);
                
                return {
                    id: stage.id,
                    title: stage.title,
                    description: stage.description,
                    sortOrder: stage.sortOrder,
                    color: stage.color || "#cccccc",
                    roleIds: roleIds,
                    isImmutable: !!stage.isImmutable // <-- Add isImmutable
                };
                });
                
                console.log("Final processed stages:", this.stages);
                
                // Convert relations from API format to UI format
                this.relations = (result.relations || []).map(function(relation) {
                return {
                    sourceStageId: relation.sourceStageId,
                    targetStageId: relation.targetStageId
                };
                });
                
                //console.log("Final processed relations:", this.relations);
                
                this.loading = false;
                
                // Initialize diagram after data is loaded with a failsafe mechanism
                this.$nextTick(() => {
                this.attemptDiagramInitialization();
                });
            },

        // Reliable diagram initialization with exponential backoff
        attemptDiagramInitialization: function() {
            if (this.diagramInitialized && this.goJsDiagram) {
                //console.log("GoJS: Diagram already initialized successfully");
                this.updateGoJsModel();
                return;
            }

            if (this.initAttempts >= this.maxInitAttempts) {
                //console.warn("GoJS: Max initialization attempts reached. Please check for errors.");
                return;
            }

            //console.log(`GoJS: Initialization attempt ${this.initAttempts + 1} of ${this.maxInitAttempts}`);
            this.initAttempts++;

            // Try to initialize the diagram
            const success = this.initGoJsDiagram();
            
            if (success && this.diagramInitialized && this.goJsDiagram) {
                //console.log("GoJS: Initialization successful");
                this.updateGoJsModel();
            } else {
                // Retry with exponential backoff
                const delay = Math.min(1000 * Math.pow(1.5, this.initAttempts), 5000);
                //console.log(`GoJS: Scheduling retry in ${delay}ms`);
                setTimeout(() => this.attemptDiagramInitialization(), delay);
            }
        },
        
        // Initializes the GoJS diagram 
        initGoJsDiagram: function() {
            //console.log("Initializing GoJS diagram...");
            
            // First check if GoJS is available globally
            if (typeof go === 'undefined') {
                //console.error("GoJS library is not loaded. Will try again later.");
                return false;
            }
            
            // Get diagram div by ID and ref (if available)
            let diagramDiv = null;
            
            // Try multiple methods to get the diagram div, in order of reliability
            if (this.$refs && this.$refs.workflowDiagramDiv) {
                diagramDiv = this.$refs.workflowDiagramDiv;
                //console.log("GoJS: Found diagram div via Vue $refs");
            } else {
                diagramDiv = document.getElementById('workflowDiagramDiv');
                if (diagramDiv) {
                    //console.log("GoJS: Found diagram div via getElementById");
                } else {
                    // Wait longer for Vue to finish rendering
                    //console.error("GoJS: Diagram div element not found in DOM");
                    return false;
                }
            }

            // Cleanup any existing diagram
            if (this.goJsDiagram) {
                //console.log("GoJS: Cleaning up old diagram instance");
                try {
                    // Make sure we're really cleaning up
                    if (this.goJsDiagram.div) {
                        this.goJsDiagram.div = null;
                    }
                    this.goJsDiagram = null;
                } catch (e) {
                    //console.error("GoJS: Error cleaning up old diagram:", e);
                }
            }

            // Ensure the diagram container is properly sized and empty
            if (diagramDiv) {
                diagramDiv.innerHTML = ''; // Clear any content
                diagramDiv.style.height = "400px";
                diagramDiv.style.minHeight = "400px";
                diagramDiv.style.width = "100%";
                diagramDiv.style.position = "relative";
                diagramDiv.style.display = "block";
                
                // Force a reflow to ensure CSS is applied
                void diagramDiv.offsetHeight;
            } else {
                //console.error("GoJS: No diagram div available after checks. Diagram initialization will fail.");
                return false;
            }

            //console.log("GoJS: Creating new diagram instance. Div height:", diagramDiv.offsetHeight);
            const $go = go.GraphObject.make;

            try {
                this.goJsDiagram = $go(go.Diagram, diagramDiv, {
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

                // Node template: both shapes are Rectangles, with matching rounded corners
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
                                    // Only round top-left and bottom-left corners
                                    parameter1: 0,
                                    geometryString: "F M0,0 H12 Q12,0 12,8 V72 Q12,80 0,80 H0 Q0,80 0,72 V8 Q0,0 0,0 Z"
                                },
                                new go.Binding("fill", "color"),
                                // Use a custom geometry for rounded left corners
                                {
                                    // This geometryString is for a rectangle with top-left and bottom-left corners rounded (radius 8)
                                    geometryString: "F M12,0 Q0,0 0,8 V72 Q0,80 12,80 H12 V0 Z"
                                }
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
                                        // Only round top-right and bottom-right corners (radius 8)
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
                                        return d ? (d.length > 50 ? d.substring(0, 50) + '...' : d) : '';
                                    })
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
                    
                this.diagramInitialized = true;
                //console.log("GoJS: Diagram initialization complete");
                return true;
            } catch (e) {
                console.error("GoJS: Error during diagram initialization:", e);
                return false;
            }
        },
        
        // Updates the GoJS model with current stages and relations
        updateGoJsModel: function() {
            //console.log("Updating GoJS model...");
            
            // Make sure GoJS library is loaded
            if (typeof go === 'undefined') {
                //console.error("GoJS: Library not loaded. Cannot update model.");
                return false;
            }
            
            // Try to initialize if not already done
            if (!this.goJsDiagram || !this.diagramInitialized) {
                //console.log("GoJS: Diagram not initialized yet, attempting initialization");
                
                // Reset initialization attempts to make sure we try again with a fresh counter
                this.initAttempts = 0;
                
                // Try to initialize diagram
                if (!this.initGoJsDiagram()) {
                    //console.error("GoJS updateGoJsModel: Diagram initialization failed. Will retry later.");
                    // Schedule another initialization attempt
                    setTimeout(() => this.attemptDiagramInitialization(), 500);
                    return false;
                }
            }

            // Double-check that diagram is available
            if (!this.goJsDiagram) {
                //console.error("GoJS updateGoJsModel: Diagram instance is not available. initGoJsDiagram might have failed or was not called when the div was ready.");
                return false;
            }

            // Handle empty stages case
            if (!this.stages || this.stages.length === 0) {
                //console.log("GoJS: No stages to display, clearing diagram");
                try {
                    // Create empty model
                    const emptyModel = new go.GraphLinksModel([], []);
                    this.goJsDiagram.model = emptyModel;
                    this.goJsDiagram.requestUpdate();
                } catch (e) {
                    //console.error("GoJS: Error clearing diagram:", e);
                }
                return true;
            }

            try {
                // Debug log about diagram
                //console.log("GoJS: Diagram div element:", this.goJsDiagram.div);
                //console.log("GoJS: Building node data array with", this.stages.length, "stages");
                
                // Create node data array
                const nodeDataArray = [];
                this.stages.forEach(stage => {
                    if (stage && stage.id) {
                        nodeDataArray.push({
                            key: stage.id,
                            title: stage.title || "Untitled stage",
                            description: stage.description || "",
                            isPublished: false, // No published info in edit mode
                            color: stage.color || "#cccccc"
                        });
                    } else {
                        //console.warn("GoJS: Skipping invalid stage:", stage);
                    }
                });

                //console.log("GoJS: Building link data array with", this.relations.length, "relations");
                const linkDataArray = [];
                
                // Use forEach instead of map for better error handling
                this.relations.forEach(rel => {
                    if (rel && rel.sourceStageId && rel.targetStageId) {
                        linkDataArray.push({
                            from: rel.sourceStageId,
                            to: rel.targetStageId
                        });
                    } else {
                        //console.warn("GoJS: Skipping invalid relation:", rel);
                    }
                });

                //console.log("GoJS: Setting model with", nodeDataArray.length, "nodes and", linkDataArray.length, "links");
                
                // Create new model
                const newModel = new go.GraphLinksModel();
                
                // Set key properties for nodes and links
                newModel.nodeKeyProperty = "key";
                newModel.linkFromKeyProperty = "from";
                newModel.linkToKeyProperty = "to";
                
                // Add data
                newModel.nodeDataArray = nodeDataArray;
                newModel.linkDataArray = linkDataArray;
                
                // Update diagram with new model
                this.goJsDiagram.model = newModel;
                
                // Request update and redraw
                this.goJsDiagram.requestUpdate();
                
                // Force layout
                if (this.goJsDiagram.layout) {
                    this.goJsDiagram.layout.invalidateLayout();
                    this.goJsDiagram.layoutDiagram();
                }
                
                return true;
            } catch (e) {
                console.error("GoJS: Error updating model:", e);
                return false;
            }
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
                        
                        // Reset initialization attempts counter on new data load
                        self.initAttempts = 0;
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
            
            // Load available roles
            fetch(this.rolesUrl)
                .then(response => response.json())
                .then(roles => {
                    this.availableRoles = roles;
                })
                .catch(error => {
                    console.error("Error loading roles:", error);
                });
        },
        
        create: function () {
            // Reset the data
            this.id = null;
            this.title = "";
            this.description = "";
            this.stages = [];
            this.relations = [];
            
            this.loading = false;
            
            // Load available roles
            fetch(this.rolesUrl)
                .then(response => response.json())
                .then(roles => {
                    this.availableRoles = roles;
                    
                    // Initialize diagram after roles are loaded and Vue updates
                    this.$nextTick(() => {
                        // Reset initialization attempts counter
                        this.initAttempts = 0;
                        this.attemptDiagramInitialization();
                    });
                })
                .catch(error => {
                    console.error("Error loading roles:", error);
                });
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
                if (!self.stages[i].title) {
                    piranha.notifications.push({
                        body: "Please enter a title for all stages",
                        type: "danger",
                        hide: true
                    });
                    return;
                }
            }

            // Check for duplicate stage titles
            var stageTitles = self.stages.map(function(stage) { return stage.title.trim().toLowerCase(); });
            var uniqueStageTitles = new Set(stageTitles);
            if (stageTitles.length !== uniqueStageTitles.size) {
                piranha.notifications.push({
                    body: "Stage titles must be unique. Please ensure no two stages have the same name.",
                    type: "danger",
                    hide: true
                });
                return;
            }

            // New validation: No isolated stages (unless it's the only stage)
            if (self.stages.length > 1) {
                for (var i = 0; i < self.stages.length; i++) {
                    var currentStage = self.stages[i];
                    var isConnected = self.relations.some(function(rel) {
                        return rel.sourceStageId === currentStage.id || rel.targetStageId === currentStage.id;
                    });
                    if (!isConnected) {
                        piranha.notifications.push({
                            body: "Stage '" + currentStage.title + "' is isolated. All stages must be connected to the workflow.",
                            type: "danger",
                            hide: true
                        });
                        return; // Stop save
                    }
                }
            }

            // New validation: All stages must be reachable from Draft (if Draft exists)
            var draftStage = self.stages.find(function(s) { return s.title.trim().toLowerCase() === "draft"; });
            if (draftStage) {
                for (var i = 0; i < self.stages.length; i++) {
                    var targetStage = self.stages[i];
                    if (targetStage.id === draftStage.id) {
                        continue; // Skip Draft itself
                    }
                    if (!self.isReachable(draftStage.id, targetStage.id, self.relations)) {
                        piranha.notifications.push({
                            body: "Stage '" + targetStage.title + "' is not reachable from 'Draft'. All stages must be reachable from the 'Draft' stage.",
                            type: "danger",
                            hide: true
                        });
                        return; // Stop save
                    }
                }
            }
            
            self.loading = true;
            
            var model = {
                id: self.id,
                title: self.title,
                description: self.description,
                stages: self.stages.map(function(stage, index) {
                    return {
                        id: stage.id,
                        title: stage.title,
                        description: stage.description,
                        sortOrder: index + 1, // Update sort order based on current position
                        color: stage.color || "#cccccc",
                        roles: (stage.roleIds || []).map(function(roleId) {
                            return {
                                roleId: roleId
                                // Let the backend handle workflowStageId assignment
                            };
                        })
                    };
                }),
                relations: self.relations.map(function(relation) {
                    return {
                        sourceStageId: relation.sourceStageId,
                        targetStageId: relation.targetStageId
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

        isReachable: function(startStageId, endStageId, relations) {
            // BFS implementation to check if endStageId is reachable from startStageId
            if (startStageId === endStageId) return true;

            var queue = [startStageId];
            var visited = new Set();
            visited.add(startStageId);

            while (queue.length > 0) {
                var currentId = queue.shift();

                var neighbors = relations
                    .filter(function(rel) { return rel.sourceStageId === currentId; })
                    .map(function(rel) { return rel.targetStageId; });

                for (var i = 0; i < neighbors.length; i++) {
                    var neighborId = neighbors[i];
                    if (neighborId === endStageId) {
                        return true;
                    }
                    if (!visited.has(neighborId)) {
                        visited.add(neighborId);
                        queue.push(neighborId);
                    }
                }
            }
            return false;
        },

        addStage: function () {
            // Check if there's an immutable stage (assumed to be last)
            const hasImmutableStage = this.stages.length > 0 && this.stages[this.stages.length - 1].isImmutable;
            
            if (hasImmutableStage) {
                // Insert before the immutable stage (second to last position)
                const insertIndex = this.stages.length - 1;
                this.stages.splice(insertIndex, 0, {
                    id: piranha.utils.generateId(),
                    title: "", 
                    description: "",
                    sortOrder: insertIndex + 1, 
                    color: "#cccccc",
                    roleIds: [],
                    isImmutable: false // New stages are not immutable
                });
                
                // Update sort orders for stages after the inserted one
                for (let i = insertIndex + 1; i < this.stages.length; i++) {
                    this.stages[i].sortOrder = i + 1;
                }
            } else {
                // Add to the end if no immutable stage exists
                this.stages.push({
                    id: piranha.utils.generateId(),
                    title: "", 
                    description: "",
                    sortOrder: this.stages.length + 1, 
                    color: "#cccccc",
                    roleIds: [],
                    isImmutable: false // New stages are not immutable
                });
            }
            
            // Update diagram
            this.$nextTick(() => {
            this.updateGoJsModel();
            });
        },
                
        removeStage: function (index) {
            //console.log(this.stages);
            const stageToRemove = this.stages[index];
            if (stageToRemove.isImmutable) {
                piranha.notifications.push({
                    body: "This stage cannot be deleted.",
                    type: "danger",
                    hide: true
                });
                return;
            }
            // Remove the stage
            this.stages.splice(index, 1);
            
            // Clean up any relations involving this stage
            this.relations = this.relations.filter(function(relation) {
                return relation.sourceStageId !== stageToRemove.id && relation.targetStageId !== stageToRemove.id;
            });
            
            // Update diagram
            this.$nextTick(() => {
                this.updateGoJsModel();
            });
        },
        
        moveStageUp: function(index) {
            if (
                index > 0 &&
                !this.stages[index].isImmutable &&
                !this.stages[index - 1].isImmutable
            ) {
                const stage = this.stages.splice(index, 1)[0];
                this.stages.splice(index - 1, 0, stage);
                this.$nextTick(() => {
                    this.updateGoJsModel();
                });
            }
        },
        
        moveStageDown: function(index) {
            if (
                index < this.stages.length - 1 &&
                !this.stages[index].isImmutable &&
                !this.stages[index + 1].isImmutable
            ) {
                const stage = this.stages.splice(index, 1)[0];
                this.stages.splice(index + 1, 0, stage);
                this.$nextTick(() => {
                    this.updateGoJsModel();
                });
            }
        },
        
        getOtherStages: function(currentStageId) {
            return this.stages.filter(function(stage) {
                return stage.id !== currentStageId;
            });
        },
        
        canTransitionTo: function(sourceStageId, targetStageId) {
            return this.relations.some(function(relation) {
                return relation.sourceStageId === sourceStageId && relation.targetStageId === targetStageId;
            });
        },
        
        isRoleEditingDisabled: function(stage) {
            if (!stage.isImmutable) {
                return false; // Not immutable, so roles are editable
            }
            // Stage is immutable. Check if it's the "Published" stage (assumed to be the last stage).
            const isPublishedStage = this.stages.length > 0 && this.stages[this.stages.length - 1].id === stage.id;
            if (isPublishedStage) {
                return true; // It's the "Published" stage, roles are not editable even if stage is immutable.
            }
            return false; // It's an immutable stage other than "Published", so roles are editable.
        },
        
        toggleStageRole: function(stage, roleId, event) {
            if (this.isRoleEditingDisabled(stage)) {
                event.preventDefault(); // Prevent checkbox state change
                // Ensure the visual state of the checkbox matches the actual state
                var checkbox = event.target;
                var intendedState = stage.roleIds && stage.roleIds.includes(roleId);
                if (checkbox.checked !== intendedState) {
                    checkbox.checked = intendedState;
                }
                // Optionally, show a notification
                // piranha.notifications.push({
                //     body: "Roles for this immutable stage cannot be modified.",
                //     type: "warning",
                //     hide: true
                // });
                return;
            }

            if (event.target.checked) {
                // Add role
                if (!stage.roleIds.includes(roleId)) {
                    stage.roleIds.push(roleId);
                }
            } else {
                // Remove role
                stage.roleIds = stage.roleIds.filter(function(id) {
                    return id !== roleId;
                });
            }
        },

        selectAllRoles: function(stage) {
            if (this.isRoleEditingDisabled(stage)) {
                return;
            }
            var self = this;
            this.availableRoles.forEach(function(role) {
                if (!stage.roleIds.includes(role.id)) {
                    stage.roleIds.push(role.id);
                }
            });
        },

        deselectAllRoles: function(stage) {
            if (this.isRoleEditingDisabled(stage)) {
                return;
            }
            stage.roleIds = [];
        },

        isRelationEditingDisabled: function(stage) {
            if (!stage.isImmutable) {
                return false; // Not immutable, so relations are editable
            }
            // Stage is immutable. Check if it's the "Published" stage (assumed to be the last stage).
            const isPublishedStage = this.stages.length > 0 && this.stages[this.stages.length - 1].id === stage.id;
            if (isPublishedStage) {
                return true; // It's the "Published" stage, roles are not editable even if stage is immutable.
            }
            return false; // It's an immutable stage other than "Published", so roles are editable.
        },
        
        toggleStageRelation: function(sourceStageId, targetStageId, event) {
            const sourceStage = this.stages.find(s => s.id === sourceStageId);

            if (this.isRelationEditingDisabled(sourceStage)) {
                event.preventDefault(); // Prevent checkbox state change
                // Ensure the visual state of the checkbox matches the actual state if it was somehow changed
                // This is a safeguard; Vue's :disabled binding should prevent this.
                var checkbox = event.target;
                var intendedState = this.canTransitionTo(sourceStageId, targetStageId);
                if (checkbox.checked !== intendedState) {
                    checkbox.checked = intendedState;
                }

                piranha.notifications.push({
                    body: "Relations for this immutable stage cannot be modified.",
                    type: "warning",
                    hide: true
                });
                return;
            }

            // Proceed with logic if not disabled
            if (event.target.checked) {
                // Add relation if it doesn't exist
                if (!this.canTransitionTo(sourceStageId, targetStageId)) {
                    this.relations.push({
                        sourceStageId: sourceStageId,
                        targetStageId: targetStageId
                    });
                }
            } else {
                // Remove relation if it exists
                this.relations = this.relations.filter(function(relation) {
                    return !(relation.sourceStageId === sourceStageId && relation.targetStageId === targetStageId);
                });
            }
            
            //console.log("Relation toggled, updating diagram");
            
            // Update diagram to reflect relation changes
            this.$nextTick(() => {
                this.updateGoJsModel();
            });
        },
        
        // Helper method to truncate description text
        truncateDescription: function(text, maxLength) {
            if (!text) return '';
            return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
        },
        onStageColorChange: function(stage, event) {
            if (stage.isImmutable) {
                event.preventDefault();
                return;
            }
            stage.color = event.target.value;
            this.$nextTick(() => {
                this.updateGoJsModel();
            });
        },
    },
    computed: {
        // Add computed properties to help with rendering
        hasStages: function() {
            return this.stages && this.stages.length > 0;
        }
    },
    updated: function() {
        // Make sure GoJS diagram is initialized when component updates
        this.$nextTick(() => {
            // If we're not in loading state and goJsDiagram is not initialized
            if (!this.loading && !this.diagramInitialized && document.getElementById('workflowDiagramDiv')) {
                //console.log("Vue updated hook: Initializing diagram");
                this.attemptDiagramInitialization();
            } else if (this.diagramInitialized && this.goJsDiagram) {
                // If the diagram is already initialized, make sure model is up-to-date
                //console.log("Vue updated hook: Diagram initialized, updating model");
                this.updateGoJsModel();
            }
        });
    },
    mounted: function() {
        //console.log("Vue component mounted");
        // Initialize diagram when component is mounted
        this.$nextTick(() => {
            // Reset initialization attempts counter
            this.initAttempts = 0;
            this.attemptDiagramInitialization();
        });
    }
});