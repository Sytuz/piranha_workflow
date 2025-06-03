/*global
    piranha
*/

piranha.pageedit = new Vue({
    el: "#pageedit",
    data: {
        loading: true,
        id: null,
        siteId: null,
        parentId: null,
        originalId: null,
        sortOrder: 0,
        typeId: null,
        title: null,
        navigationTitle: null,
        slug: null,
        metaTitle: null,
        metaKeywords: null,
        metaDescription: null,
        metaIndex: null,
        metaFollow: null,
        metaPriority: null,
        ogTitle: null,
        ogDescription: null,
        ogImage: {
            id: null,
            media: null
        },
        excerpt: null,
        isHidden: false,
        published: null,
        publishedTime: null,
        redirectUrl: null,
        redirectType: null,
        enableComments: null,
        closeCommentsAfterDays: null,
        commentCount: null,
        pendingCommentCount: 0,
        state: "new",
        blocks: [],
        regions: [],
        editors: [],
        useBlocks: true,
        usePrimaryImage: true,
        useExcerpt: true,
        useHtmlExcerpt: true,
        permissions: [],
        primaryImage: {
            id: null,
            media: null
        },
        selectedPermissions: [],
        isCopy: false,
        isScheduled: false,
        saving: false,
        savingDraft: false,
        selectedRegion: {
            uid: "uid-blocks",
            name: null,
            icon: null,
        },
        selectedSetting: "uid-settings",
        selectedRoute: null,
        routes: [],
        changeRequestTitle: "",
        changeRequestNotes: "",
        changeRequestTitleError: null,
        submittingChangeRequest: false,
        
        // Workflow visualization properties
        workflowData: null,
        accessibleStages: [],
        selectedTargetStage: "",
        targetStageError: null,
        workflowDiagram: null,
        loadingWorkflow: false
    },
    computed: {
        contentRegions: function () {
            return this.regions.filter(function (item) {
                return item.meta.display != "setting" && item.meta.display != "hidden";
            });
        },
        settingRegions: function () {
            return this.regions.filter(function (item) {
                return item.meta.display === "setting";
            });
        },
        primaryImageUrl: function () {
            if (this.primaryImage.media != null) {
                return piranha.utils.formatUrl("~/manager/api/media/url/" + this.primaryImage.id + "/448/200");
                //return piranha.utils.formatUrl(this.primaryImage.media.publicUrl);
            } else {
                return piranha.utils.formatUrl("~/manager/assets/img/empty-image.png");
            }
        },
        isExcerptEmpty: function () {
            return piranha.utils.isEmptyText(this.excerpt);
        },
        metaPriorityDescription: function() {
            var description = piranha.resources.texts.important;
            if (this.metaPriority <= 0.3)
                description = piranha.resources.texts.low;
            else if (this.metaPriority <= 0.6)
                description =  piranha.resources.texts.medium;
            else if (this.metaPriority <= 0.9)
                description =  piranha.resources.texts.high;

            return description += " (" + this.metaPriority + ")";
        }
    },
    mounted() {
        document.addEventListener("keydown", this.doHotKeys);
    },
    beforeDestroy() {
        document.removeEventListener("keydown", this.doHotKeys);
    },
    methods: {
        bind: function (model) {
            this.id = model.id;
            this.siteId = model.siteId;
            this.parentId = model.parentId;
            this.originalId = model.originalId;
            this.sortOrder = model.sortOrder;
            this.typeId = model.typeId;
            this.title = model.title;
            this.navigationTitle = model.navigationTitle;
            this.slug = model.slug;
            this.metaTitle = model.metaTitle;
            this.metaKeywords = model.metaKeywords;
            this.metaDescription = model.metaDescription;
            this.metaIndex = model.metaIndex;
            this.metaFollow = model.metaFollow;
            this.metaPriority = model.metaPriority;
            this.ogTitle = model.ogTitle;
            this.ogDescription = model.ogDescription;
            this.ogImage = model.ogImage;
            this.excerpt = model.excerpt;
            this.isHidden = model.isHidden;
            this.published = model.published;
            this.publishedTime = model.publishedTime;
            this.redirectUrl = model.redirectUrl;
            this.redirectType = model.redirectType;
            this.enableComments = model.enableComments;
            this.closeCommentsAfterDays = model.closeCommentsAfterDays;
            this.commentCount = model.commentCount;
            this.pendingCommentCount = model.pendingCommentCount;
            this.state = model.state;
            this.blocks = model.blocks;
            this.regions = model.regions;
            this.editors = model.editors;
            this.useBlocks = model.useBlocks;
            this.usePrimaryImage = model.usePrimaryImage;
            this.useExcerpt = model.useExcerpt;
            this.useHtmlExcerpt = model.useHtmlExcerpt;
            this.isCopy = model.isCopy;
            this.isScheduled = model.isScheduled;
            this.selectedRoute = model.selectedRoute;
            this.routes = model.routes;
            this.permissions = model.permissions;
            this.primaryImage = model.primaryImage;
            this.selectedPermissions = model.selectedPermissions;

            if (!this.useBlocks) {
                // First choice, select the first custom editor
                if (this.editors.length > 0) {
                    this.selectedRegion = this.editors[0];
                }

                // Second choice, select the first content region
                else if (this.contentRegions.length > 0) {
                    this.selectedRegion = this.contentRegions[0].meta;
                }
            } else {
                this.selectedRegion = {
                    uid: "uid-blocks",
                    name: null,
                    icon: null,
                };
            }
        },
        load: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        create: function (id, pageType) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/create/" + id + "/" + pageType)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        createrelative: function (id, pageType, after) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/createrelative/" + id + "/" + pageType + "/" + after)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        copy: function (source, siteId) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/copy/" + source + "/" + siteId)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        copyrelative: function (source, id, after) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/copyrelative/" + source + "/" + id + "/" + after)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        doHotKeys(e)
        {
            // CTRL + S
            if (e.keyCode === 83 && e.ctrlKey)
            {
                e.preventDefault();
                this.saveDraft();
            }
        },
        save: function ()
        {
            this.saving = true;
            this.saveInternal(piranha.baseUrl + "manager/api/page/save");
        },
        saveDraft: function ()
        {
            this.savingDraft = true;
            this.saveInternal(piranha.baseUrl + "manager/api/page/save/draft");
        },
        unpublish: function ()
        {
            this.saving = true;
            this.saveInternal(piranha.baseUrl + "manager/api/page/save/unpublish");
        },
        saveInternal: function (route) {
            var self = this;

            var model = {
                id: self.id,
                siteId: self.siteId,
                parentId: self.parentId,
                originalId: self.originalId,
                sortOrder: self.sortOrder,
                typeId: self.typeId,
                title: self.title,
                navigationTitle: self.navigationTitle,
                slug: self.slug,
                metaTitle: self.metaTitle,
                metaKeywords: self.metaKeywords,
                metaDescription: self.metaDescription,
                metaIndex: self.metaIndex,
                metaFollow: self.metaFollow,
                metaPriority: self.metaPriority,
                ogTitle: self.ogTitle,
                ogDescription: self.ogDescription,
                ogImage: {
                    id: self.ogImage.id
                },
                excerpt: self.excerpt,
                isHidden: self.isHidden,
                published: self.published,
                publishedTime: self.publishedTime,
                redirectUrl: self.redirectUrl,
                redirectType: self.redirectType,
                enableComments: self.enableComments,
                closeCommentsAfterDays: self.closeCommentsAfterDays,
                isCopy: self.isCopy,
                blocks: JSON.parse(JSON.stringify(self.blocks)),
                regions: JSON.parse(JSON.stringify(self.regions)),
                selectedRoute: self.selectedRoute,
                selectedPermissions: self.selectedPermissions,
                primaryImage: {
                    id: self.primaryImage.id
                },
            };

            fetch(route, {
                method: "post",
                headers: piranha.utils.antiForgeryHeaders(),
                body: JSON.stringify(model)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                var oldState = self.state;

                self.id = result.id;
                self.slug = result.slug;
                self.published = result.published;
                self.publishedTime = result.publishedTime;
                self.state = result.state;
                self.isCopy = result.isCopy;
                self.selectedRoute = result.selectedRoute;

                if (oldState === 'new' && result.state !== 'new') {
                    window.history.replaceState({ state: "created"}, "Edit page", piranha.baseUrl + "manager/page/edit/" + result.id);
                }
                piranha.notifications.push(result.status);

                self.saving = false;
                self.savingDraft = false;

                self.eventBus.$emit("onSaved", self.state)
            })
            .catch(function (error) {
                console.log("error:", error);
            });
        },
        revert: function () {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/revert", {
                method: "post",
                headers: piranha.utils.antiForgeryHeaders(),
                body: JSON.stringify(self.id)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                self.bind(result);

                piranha.notifications.push(result.status);
            })
            .catch(function (error) { 
                console.log("error:", error );
            });
        },
        detach: function () {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/detach", {
                method: "post",
                headers: piranha.utils.antiForgeryHeaders(),
                body: JSON.stringify(self.id)
            })
            .then(function (response) { return response.json(); })
            .then(function (result) {
                self.bind(result);

                piranha.notifications.push(result.status);
            })
            .catch(function (error) { 
                console.log("error:", error );
            });
        },
        remove: function () {
            var self = this;

            piranha.alert.open({
                title: piranha.resources.texts.delete,
                body: piranha.resources.texts.deletePageConfirm,
                confirmCss: "btn-danger",
                confirmIcon: "fas fa-trash",
                confirmText: piranha.resources.texts.delete,
                onConfirm: function () {
                    fetch(piranha.baseUrl + "manager/api/page/delete", {
                        method: "delete",
                        headers: piranha.utils.antiForgeryHeaders(),
                        body: JSON.stringify(self.id)
                    })
                    .then(function (response) { return response.json(); })
                    .then(function (result) {
                        piranha.notifications.push(result);

                        window.location = piranha.baseUrl + "manager/pages";
                    })
                    .catch(function (error) { console.log("error:", error ); });
                }
            });
        },
        addBlock: function (type, pos) {
            fetch(piranha.baseUrl + "manager/api/content/block/" + type)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    piranha.pageedit.blocks.splice(pos, 0, result.body);
                })
                .catch(function (error) { console.log("error:", error );
            });
        },
        moveBlock: function (from, to) {
            this.blocks.splice(to, 0, this.blocks.splice(from, 1)[0])
        },
        collapseBlock: function (block) {
            block.meta.isCollapsed = !block.meta.isCollapsed;
        },
        removeBlock: function (block) {
            var index = this.blocks.indexOf(block);

            if (index !== -1) {
                this.blocks.splice(index, 1);
            }
        },
        updateBlockTitle: function (e) {
            for (var n = 0; n < this.blocks.length; n++) {
                if (this.blocks[n].meta.uid === e.uid) {
                    this.blocks[n].meta.title = e.title;
                    break;
                }
            }
        },
        selectRegion: function (region) {
            this.selectedRegion = region;
            Vue.nextTick(function () {
                piranha.editor.refreshMarkdown();
            });
        },
        selectSetting: function (uid) {
            this.selectedSetting = uid;
            Vue.nextTick(function () {
                piranha.editor.refreshMarkdown();
            });
        },
        isCommentsOpen: function () {
            var date = new Date(this.published + " " + this.publishedTime);
            date = date.addDays(this.closeCommentsAfterDays);

            return date > new Date();
        },
        commentsClosedDate: function () {
            var date = new Date(this.published + " " + this.publishedTime);
            date = date.addDays(this.closeCommentsAfterDays);

            return date.toDateString();
        },
        selectPrimaryImage: function () {
            if (this.primaryImage.media !== null) {
                piranha.mediapicker.open(this.updatePrimaryImage, "Image", this.primaryImage.media.folderId);
            } else {
                piranha.mediapicker.openCurrentFolder(this.updatePrimaryImage, "Image");
            }
        },
        removePrimaryImage: function () {
            this.primaryImage.id = null;
            this.primaryImage.media = null;
        },
        updatePrimaryImage: function (media) {
            if (media.type === "Image") {
                this.primaryImage.id = media.id;
                this.primaryImage.media = media;
            } else {
                console.log("No image was selected");
            }
        },
        onExcerptBlur: function (e) {
            if (this.useHtmlExcerpt) {
                this.excerpt = tinyMCE.activeEditor.getContent();
            } else {
                this.excerpt = e.target.innerHTML;
            }
        },        openChangeRequestModal: function () {
            // Reset form fields
            this.changeRequestTitle = "";
            this.changeRequestNotes = "";
            this.changeRequestTitleError = null;
            this.targetStageError = null;
            this.submittingChangeRequest = false;
            this.selectedTargetStage = "";
            this.accessibleStages = [];
            this.workflowData = null;
            
            // Load workflow data before opening modal
            this.loadWorkflowData().then(() => {
                // Open modal after workflow data is loaded
                $("#changeRequestModal").modal("show");
                
                // Initialize diagram after modal is shown and DOM is ready
                this.$nextTick(() => {
                    this.initChangeRequestDiagram();
                });
            });
        },        submitChangeRequest: function () {
            var self = this;
            // Validate form
            this.changeRequestTitleError = null;
            this.targetStageError = null;

            if (!this.changeRequestTitle || this.changeRequestTitle.trim() === "") {
                this.changeRequestTitleError = "Title is required";
                return;
            }

            // Validate target stage selection
            if (!this.selectedTargetStage) {
                this.targetStageError = "Please select a target stage";
                return;
            }

            // Ensure required fields are present and valid
            var workflowId = window.changeRequestConfig ? window.changeRequestConfig.workflowId : null;
            var createdById = window.changeRequestConfig ? window.changeRequestConfig.userId : null;

            if (!workflowId) {
                this.changeRequestTitleError = "Workflow is required";
                return;
            }
            if (!createdById) {
                this.changeRequestTitleError = "User is required";
                return;
            }

            // Use excerpt, page/post title, or change request title as content
            var content = "";
            if (this.excerpt && this.excerpt.trim() !== "") {
                content = this.excerpt.trim();
            } else if (this.title && this.title.trim() !== "") {
                content = this.title.trim();
            } else if (this.changeRequestTitle && this.changeRequestTitle.trim() !== "") {
                content = this.changeRequestTitle.trim();
            }

            var contentSnapshot = JSON.stringify({
                title: this.title,
                blocks: this.blocks,
                regions: this.regions,
                excerpt: this.excerpt,
                // add other fields as needed
            });

            if (!contentSnapshot || contentSnapshot === "{}") {
                this.changeRequestTitleError = "Content snapshot is required";
                return;
            }            var changeRequest = {
                ContentId: this.id,
                Title: this.changeRequestTitle.trim(),
                Notes: this.changeRequestNotes.trim(),
                WorkflowId: workflowId,
                CreatedById: createdById,
                TargetStageId: this.selectedTargetStage,
                ContentSnapshot: contentSnapshot
            };

            fetch(piranha.baseUrl + "manager/api/changerequest/create", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    ...piranha.utils.antiForgeryHeaders()
                },
                credentials: "same-origin",
                body: JSON.stringify(changeRequest)
            })
            .then(function (response) {
                if (response.ok) {
                    return response.json();
                } else {
                    throw new Error("Failed to create change request");
                }
            })
            .then(function (result) {
                self.submittingChangeRequest = false;
                $("#changeRequestModal").modal("hide");
                piranha.notifications.push({
                    body: "Change request submitted successfully",
                    type: "success"
                });
            })
            .catch(function (error) {
                self.submittingChangeRequest = false;
                console.log("error:", error);
                piranha.notifications.push({
                    body: "Failed to submit change request. Please try again.",
                    type: "danger"
                });
            });
        },
        
        // Workflow visualization methods
        loadWorkflowData: function () {
            var self = this;
            self.loadingWorkflow = true;
            
            return fetch(piranha.baseUrl + "manager/api/workflow/enabled")
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error("Failed to load workflow data");
                    }
                    return response.json();
                })
                .then(function (workflowData) {
                    self.workflowData = workflowData;
                    self.loadingWorkflow = false;
                    
                    // Filter accessible stages from Draft stage
                    if (workflowData && workflowData.stages && workflowData.relations) {
                        self.filterAccessibleStages();
                    }
                })
                .catch(function (error) {
                    console.error("Error loading workflow data:", error);
                    self.loadingWorkflow = false;
                    self.workflowData = null;
                });
        },
        
        filterAccessibleStages: function () {
            if (!this.workflowData || !this.workflowData.stages || !this.workflowData.relations) {
                this.accessibleStages = [];
                return;
            }
            
            // Find the Draft stage
            var draftStage = this.workflowData.stages.find(function(stage) {
                return stage.title && stage.title.toLowerCase() === 'draft';
            });
            
            if (!draftStage) {
                // If no Draft stage found, show all stages
                this.accessibleStages = this.workflowData.stages.slice();
                return;
            }
            
            // Find all stages accessible from Draft using relations
            var accessibleStageIds = new Set();
            var relations = this.workflowData.relations || [];
            
            // Add stages directly accessible from Draft
            relations.forEach(function(relation) {
                if (relation.sourceStageId === draftStage.id) {
                    accessibleStageIds.add(relation.targetStageId);
                }
            });
            
            // Filter stages to only include accessible ones
            this.accessibleStages = this.workflowData.stages.filter(function(stage) {
                return accessibleStageIds.has(stage.id);
            });
        },
        
        initChangeRequestDiagram: function () {
            if (!this.workflowData || !this.workflowData.stages || this.workflowData.stages.length === 0) {
                return;
            }
            
            // Check if GoJS is available
            if (typeof go === 'undefined') {
                console.error("GoJS library not found for change request diagram");
                return;
            }
            
            var diagramDiv = document.getElementById("changeRequestWorkflowDiagram");
            if (!diagramDiv) {
                console.error("Change request diagram container not found");
                return;
            }
            
            // Initialize GoJS diagram
            this.workflowDiagram = go.GraphObject.make(go.Diagram, diagramDiv, {
                layout: go.GraphObject.make(go.LayeredDigraphLayout, {
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
              // Define node template (exact same as workflow.js with color bar)
            var $go = go.GraphObject.make;
            this.workflowDiagram.nodeTemplate =
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
            this.workflowDiagram.linkTemplate =
                go.GraphObject.make(go.Link,
                    { routing: go.Link.AvoidsNodes, corner: 10 },
                    go.GraphObject.make(go.Shape, { strokeWidth: 2, stroke: "#555" }),
                    go.GraphObject.make(go.Shape, { toArrow: "Standard", fill: "#555", stroke: null })
                );
            
            this.updateChangeRequestDiagram();
        },
        
        updateChangeRequestDiagram: function () {
            if (!this.workflowDiagram || !this.workflowData) {
                return;
            }
              // Build node data
            var nodeDataArray = this.workflowData.stages.map(function(stage) {
                return {
                    key: stage.id,
                    title: stage.title,
                    description: stage.description || "",
                    color: stage.color || "#cccccc"
                };
            });
            
            // Build link data
            var linkDataArray = (this.workflowData.relations || []).map(function(relation) {
                return {
                    from: relation.sourceStageId,
                    to: relation.targetStageId
                };
            });
            
            // Set the model
            this.workflowDiagram.model = new go.GraphLinksModel(nodeDataArray, linkDataArray);
            this.workflowDiagram.requestUpdate();
        },
    },
    created: function () {
    },
    updated: function () {
        if (this.loading)
        {
            sortable("#content-blocks", {
                handle: ".handle",
                items: ":not(.unsortable)"
            })[0].addEventListener("sortupdate", function (e) {
                piranha.pageedit.moveBlock(e.detail.origin.index, e.detail.destination.index);
            });
            piranha.editor.addInline('excerpt-body', 'excerpt-toolbar');
        }
        else {
            sortable("#content-blocks", "disable");
            sortable("#content-blocks", "enable");
        }

        this.loading = false;
    },
    components: {
        datepicker: vuejsDatepicker
    }
});
