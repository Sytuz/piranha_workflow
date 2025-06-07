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
        routes: [],        changeRequestTitle: "",
        changeRequestNotes: "",
        changeRequestTitleError: null,
        submittingChangeRequest: false,
        
        // First save change request properties
        firstSaveChangeRequestTitle: "",
        firstSaveChangeRequestNotes: "",
        firstSaveChangeRequestTitleError: null,
        submittingFirstSaveChangeRequest: false,
        pendingFirstSave: false,
        pendingFirstSaveData: null,
        
        // Workflow visualization properties
        workflowData: null,
        accessibleStages: [],
        selectedTargetStage: "",
        targetStageError: null,
        loadingWorkflow: false,
        workflowDiagram: null,
        
        // Change request version management
        changeRequests: [],
        selectedChangeRequestId: null,
        loadingChangeRequests: false,
        isEditingChangeRequest: false
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
        },
        // Determine if current change request is in read-only mode
        isChangeRequestReadOnly: function() {
            if (!this.isEditingChangeRequest || !this.selectedChangeRequestId) {
                return false;
            }
            
            var currentChangeRequest = this.changeRequests.find(function(cr) {
                return cr.id === this.selectedChangeRequestId;
            }.bind(this));
            
            if (!currentChangeRequest) {
                return false;
            }
            
            // Change request is read-only if status is not Draft (0)
            // Status enum: Draft=0, Submitted=1, InReview=2, Approved=3, Rejected=4, Published=5
            return currentChangeRequest.status !== 0; // Not Draft
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
        },        load: function (id) {
            var self = this;

            fetch(piranha.baseUrl + "manager/api/page/" + id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                    
                    // Load change requests for this page after binding the page data
                    if (self.id && self.state !== 'new') {
                        self.loadChangeRequests();
                    }
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
        },        save: function ()
        {
            this.saving = true;
            this.saveInternal(piranha.baseUrl + "manager/api/page/save");
        },        saveDraft: function ()
        {
            // Check if we're editing an existing change request
            if (this.isEditingChangeRequest && this.selectedChangeRequestId) {
                this.updateChangeRequest();
            } else {
                // Create new change request
                this.openFirstSaveChangeRequestModal();
            }
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
                self.selectedRoute = result.selectedRoute;                if (oldState === 'new' && result.state !== 'new') {
                    window.history.replaceState({ state: "created"}, "Edit page", piranha.baseUrl + "manager/page/edit/" + result.id);
                    
                    // Check if we should show the first save change request modal
                    self.checkAndShowFirstSaveModal();
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
            var self = this;
            
            // Reset error fields
            this.changeRequestTitleError = null;
            this.targetStageError = null;
            this.submittingChangeRequest = false;
            this.accessibleStages = [];
            this.workflowData = null;
            
            // Check if we have a selected change request to populate the form
            if (this.selectedChangeRequestId && this.changeRequests.length > 0) {
                var currentChangeRequest = this.changeRequests.find(function(cr) {
                    return cr.id === self.selectedChangeRequestId;
                });
                
                if (currentChangeRequest) {
                    // Populate form with current change request data
                    this.changeRequestTitle = currentChangeRequest.title || "";
                    this.changeRequestNotes = currentChangeRequest.notes || "";
                    this.selectedTargetStage = currentChangeRequest.stageId || "";
                } else {
                    // Reset form fields if change request not found
                    this.changeRequestTitle = "";
                    this.changeRequestNotes = "";
                    this.selectedTargetStage = "";
                }
            } else {
                // Reset form fields for new change request
                this.changeRequestTitle = "";
                this.changeRequestNotes = "";
                this.selectedTargetStage = "";
            }
            
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

            // Create content snapshot
            var contentSnapshot = JSON.stringify({
                title: this.title,
                blocks: this.blocks,
                regions: this.regions,
                excerpt: this.excerpt
            });

            if (!contentSnapshot || contentSnapshot === "{}") {
                this.changeRequestTitleError = "Content snapshot is required";
                return;
            }

            this.submittingChangeRequest = true;

            // Check if we're updating an existing change request or creating a new one
            if (this.selectedChangeRequestId && this.changeRequests.length > 0) {
                // Update existing change request
                var currentChangeRequest = this.changeRequests.find(function(cr) {
                    return cr.id === self.selectedChangeRequestId;
                });

                if (currentChangeRequest) {
                    var updateData = {
                        Id: currentChangeRequest.id,
                        Title: this.changeRequestTitle.trim(),
                        Notes: this.changeRequestNotes.trim(),
                        ContentSnapshot: contentSnapshot,
                        WorkflowId: currentChangeRequest.workflowId,
                        ContentId: currentChangeRequest.contentId,
                        StageId: this.selectedTargetStage,
                        CreatedById: currentChangeRequest.createdById,
                        Status: currentChangeRequest.status,
                        CreatedAt: currentChangeRequest.created,
                        LastModified: new Date().toISOString()
                    };

                    fetch(piranha.baseUrl + "manager/api/changerequest/save", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            ...piranha.utils.antiForgeryHeaders()
                        },
                        credentials: "same-origin",
                        body: JSON.stringify(updateData)
                    })
                    .then(function (response) {
                        if (response.ok) {
                            return response.json();
                        } else {
                            throw new Error("Failed to update change request");
                        }
                    })                    .then(function (result) {
                        self.submittingChangeRequest = false;
                        $("#changeRequestModal").modal("hide");
                        
                        // Update the local change request data
                        currentChangeRequest.title = self.changeRequestTitle.trim();
                        currentChangeRequest.notes = self.changeRequestNotes.trim();
                        currentChangeRequest.contentSnapshot = contentSnapshot;
                        currentChangeRequest.stageId = self.selectedTargetStage;
                        // Update status to Submitted when change request is submitted
                        currentChangeRequest.status = 1; // Submitted status
                        
                        piranha.notifications.push({
                            body: "Change request submitted successfully",
                            type: "success"
                        });
                        
                        // Reload change requests to get updated data from server
                        self.loadChangeRequests();
                    })
                    .catch(function (error) {
                        self.submittingChangeRequest = false;
                        console.log("error:", error);
                        piranha.notifications.push({
                            body: "Failed to update change request. Please try again.",
                            type: "danger"
                        });
                    });
                } else {
                    this.submittingChangeRequest = false;
                    piranha.notifications.push({
                        body: "Selected change request not found",
                        type: "danger"
                    });
                }
            } else {
                // Create new change request
                var changeRequest = {
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
                    
                    // Now save the page after change request is successfully created
                    self.savingDraft = true;
                    self.saveInternal(piranha.baseUrl + "manager/api/page/save/draft");
                    
                    piranha.notifications.push({
                        body: "Change request created successfully",
                        type: "success"
                    });
                    
                    // Reload change requests to include the new one
                    self.loadChangeRequests();
                })
                .catch(function (error) {
                    self.submittingChangeRequest = false;
                    console.log("error:", error);
                    piranha.notifications.push({
                        body: "Failed to create change request. Please try again.",
                        type: "danger"
                    });
                });
            }
        },
        
        // First save change request methods
        checkAndShowFirstSaveModal: function () {
            // Check if we have workflow configuration and this is a new page save
            var workflowId = window.changeRequestConfig ? window.changeRequestConfig.workflowId : null;
            if (workflowId) {
                this.openFirstSaveChangeRequestModal();
            }
        },
        
        openFirstSaveChangeRequestModal: function () {
            // Reset form fields
            this.firstSaveChangeRequestTitle = this.title || ""; // Pre-fill with page title
            this.firstSaveChangeRequestNotes = "";
            this.firstSaveChangeRequestTitleError = null;
            this.submittingFirstSaveChangeRequest = false;
            
            // Show modal
            $("#firstSaveChangeRequestModal").modal("show");
        },
        
        submitFirstSaveChangeRequest: function () {
            var self = this;
            
            // Validate form
            this.firstSaveChangeRequestTitleError = null;
            
            if (!this.firstSaveChangeRequestTitle || this.firstSaveChangeRequestTitle.trim() === "") {
                this.firstSaveChangeRequestTitleError = "Title is required";
                return;
            }
            
            this.submittingFirstSaveChangeRequest = true;
            
            // Get workflow and user configuration
            var workflowId = window.changeRequestConfig ? window.changeRequestConfig.workflowId : null;
            var userId = window.changeRequestConfig ? window.changeRequestConfig.userId : null;
            
            if (!workflowId) {
                this.submittingFirstSaveChangeRequest = false;
                piranha.notifications.push({
                    body: "No workflow configuration found",
                    type: "danger"
                });
                return;
            }
            
            if (!userId) {
                this.submittingFirstSaveChangeRequest = false;
                piranha.notifications.push({
                    body: "User configuration not found",
                    type: "danger"
                });
                return;
            }
            
            // First, get the Draft stage ID from the workflow
            this.getDraftStageId(workflowId).then(function(draftStageId) {
                if (!draftStageId) {
                    self.submittingFirstSaveChangeRequest = false;
                    piranha.notifications.push({
                        body: "Draft stage not found in workflow",
                        type: "danger"
                    });
                    return;
                }
                  // Prepare content snapshot as JSON
                var contentSnapshot = JSON.stringify({
                    title: self.title,
                    blocks: self.blocks,
                    regions: self.regions,
                    excerpt: self.excerpt
                });
                
                var changeRequestModel = {
                    Title: self.firstSaveChangeRequestTitle.trim(),
                    Notes: self.firstSaveChangeRequestNotes.trim(),
                    ContentSnapshot: contentSnapshot,
                    CreatedById: userId,
                    WorkflowId: workflowId,
                    TargetStageId: draftStageId,
                    ContentId: self.id,
                    ContentType: "Page"
                };
                
                // Submit change request
                fetch(piranha.baseUrl + "manager/api/changerequest/create", {
                    method: "post",
                    headers: piranha.utils.antiForgeryHeaders(),
                    body: JSON.stringify(changeRequestModel)
                })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error("Failed to create change request");
                    }
                    return response.json();
                })                .then(function (result) {
                    self.submittingFirstSaveChangeRequest = false;
                    $("#firstSaveChangeRequestModal").modal("hide");
                    
                    piranha.notifications.push({
                        body: "Change request created successfully and placed in Draft stage",
                        type: "success"
                    });
                      // Now save the page and then reload
                    self.savingDraft = true;
                    self.saveInternalWithReload(piranha.baseUrl + "manager/api/page/save/draft");
                })
                .catch(function (error) {
                    self.submittingFirstSaveChangeRequest = false;
                    console.log("error:", error);
                    piranha.notifications.push({
                        body: "Failed to create change request. Please try again.",
                        type: "danger"
                    });
                });
            }).catch(function(error) {
                self.submittingFirstSaveChangeRequest = false;
                console.log("error getting draft stage:", error);
                piranha.notifications.push({
                    body: "Failed to find Draft stage in workflow",
                    type: "danger"
                });
            });
        },
        
        getDraftStageId: function(workflowId) {
            // Fetch workflow data to get the Draft stage ID
            return fetch(piranha.baseUrl + "manager/api/workflow/" + workflowId)
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error("Failed to load workflow data");
                    }
                    return response.json();
                })
                .then(function (workflowData) {
                    // Find the Draft stage
                    var draftStage = workflowData.stages.find(function(stage) {
                        return stage.title && stage.title.toLowerCase() === 'draft';
                    });
                    
                    return draftStage ? draftStage.id : null;
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
        
        // Change request version management methods
        loadChangeRequests: function () {
            var self = this;
            
            if (!self.id || self.id === null) {
                return;
            }
            
            self.loadingChangeRequests = true;
              fetch(piranha.baseUrl + "manager/api/changerequest/content/" + self.id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.changeRequests = result || [];
                    self.loadingChangeRequests = false;
                    
                    // Automatically select the most recent change request (first in the list)
                    if (self.changeRequests.length > 0) {
                        // Sort by creation date to ensure we get the most recent one first
                        self.changeRequests.sort(function(a, b) {
                            return new Date(b.created) - new Date(a.created);
                        });
                        
                        // Select the most recent change request
                        self.selectedChangeRequestId = self.changeRequests[0].id;
                        self.loadChangeRequestVersion(self.selectedChangeRequestId);
                    }
                })
                .catch(function (error) {
                    console.log("Error loading change requests:", error);
                    self.changeRequests = [];
                    self.loadingChangeRequests = false;
                });
        },
        
        switchVersion: function () {
            var self = this;
            
            if (self.loadingChangeRequests) {
                return;
            }
            
            if (!self.selectedChangeRequestId || self.selectedChangeRequestId === "") {
                // Switch to published version
                self.loadPublishedVersion();
            } else {
                // Switch to change request version
                self.loadChangeRequestVersion(self.selectedChangeRequestId);
            }
        },
        
        loadPublishedVersion: function () {
            var self = this;
            
            self.isEditingChangeRequest = false;
            
            // Reload the original page data
            fetch(piranha.baseUrl + "manager/api/page/" + self.id)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.bind(result);
                    
                    piranha.notifications.push({
                        body: "Switched to published version",
                        type: "info"
                    });
                })
                .catch(function (error) {
                    console.log("Error loading published version:", error);
                    piranha.notifications.push({
                        body: "Failed to load published version",
                        type: "danger"
                    });
                });
        },          loadChangeRequestVersion: function (changeRequestId) {
            var self = this;
            
            self.isEditingChangeRequest = true;
            
            // Find the change request
            var changeRequest = self.changeRequests.find(function(cr) {
                return cr.id === changeRequestId;
            });
            
            if (!changeRequest) {
                piranha.notifications.push({
                    body: "Change request not found",
                    type: "danger"
                });
                self.isEditingChangeRequest = false;
                self.selectedChangeRequestId = null;
                return;
            }
            
            try {
                // Check if contentSnapshot exists and is not empty
                if (!changeRequest.contentSnapshot || changeRequest.contentSnapshot.trim() === "") {
                    piranha.notifications.push({
                        body: "Change request has no content snapshot. Staying on current version.",
                        type: "warning"
                    });
                    self.isEditingChangeRequest = false;
                    self.selectedChangeRequestId = null;
                    return;
                }
                
                var contentSnapshot;
                
                // Try to parse as JSON first
                try {
                    contentSnapshot = JSON.parse(changeRequest.contentSnapshot);
                } catch (parseError) {
                    // If JSON parsing fails, check if it's a plain string (legacy format)
                    var trimmedContent = changeRequest.contentSnapshot.trim();
                    
                    // Check if it looks like it might be intended as JSON but malformed
                    if (trimmedContent.startsWith('{') || trimmedContent.startsWith('[')) {
                        console.log("Malformed JSON detected in change request:", parseError);
                        piranha.notifications.push({
                            body: "Change request contains corrupted data. Cannot load content.",
                            type: "danger"
                        });
                        self.isEditingChangeRequest = false;
                        self.selectedChangeRequestId = null;
                        return;
                    }
                    
                    // For plain string content (legacy), we can't reconstruct the full page data
                    console.log("Legacy string content detected:", trimmedContent);
                    piranha.notifications.push({
                        body: "Change request contains legacy text format. Full content preview not available.",
                        type: "warning"
                    });
                    self.isEditingChangeRequest = false;
                    self.selectedChangeRequestId = null;
                    return;
                }
                
                // Apply the snapshot data to current page
                if (contentSnapshot.title !== undefined) {
                    self.title = contentSnapshot.title;
                }
                if (contentSnapshot.blocks !== undefined) {
                    self.blocks = contentSnapshot.blocks;
                }
                if (contentSnapshot.regions !== undefined) {
                    self.regions = contentSnapshot.regions;
                }
                if (contentSnapshot.excerpt !== undefined) {
                    self.excerpt = contentSnapshot.excerpt;
                }
                
                piranha.notifications.push({
                    body: "Switched to change request: " + changeRequest.title,
                    type: "info"
                });
                
            } catch (error) {
                console.log("Unexpected error loading change request content:", error);
                piranha.notifications.push({
                    body: "Failed to load change request content: " + error.message,
                    type: "danger"
                });
                
                // Reset to not editing mode
                self.isEditingChangeRequest = false;
                self.selectedChangeRequestId = null;
            }
        },
          formatStage: function (stageTitle) {
            return stageTitle || "Unknown";
        },
          updateChangeRequest: function () {
            var self = this;
            
            if (!self.selectedChangeRequestId) {
                piranha.notifications.push({
                    body: "No change request selected to update",
                    type: "danger"
                });
                return;
            }
            
            // Find the current change request
            var currentChangeRequest = self.changeRequests.find(function(cr) {
                return cr.id === self.selectedChangeRequestId;
            });
            
            if (!currentChangeRequest) {
                piranha.notifications.push({
                    body: "Change request not found",
                    type: "danger"
                });
                return;
            }
            
            // Create updated content snapshot
            var updatedContentSnapshot = JSON.stringify({
                title: self.title,
                blocks: self.blocks,
                regions: self.regions,
                excerpt: self.excerpt
            });
              // Prepare the change request update data (matching ChangeRequest model properties)
            var updateData = {
                Id: currentChangeRequest.id,
                Title: currentChangeRequest.title,
                Notes: currentChangeRequest.notes,
                ContentSnapshot: updatedContentSnapshot,
                WorkflowId: currentChangeRequest.workflowId,
                ContentId: currentChangeRequest.contentId,
                StageId: currentChangeRequest.stageId,
                CreatedById: currentChangeRequest.createdById,
                Status: currentChangeRequest.status,
                CreatedAt: currentChangeRequest.created,
                LastModified: new Date().toISOString()
            };
            
            self.savingDraft = true;
            
            fetch(piranha.baseUrl + "manager/api/changerequest/save", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    ...piranha.utils.antiForgeryHeaders()
                },
                credentials: "same-origin",
                body: JSON.stringify(updateData)
            })
            .then(function (response) {
                if (response.ok) {
                    return response.json();
                } else {
                    throw new Error("Failed to update change request");
                }
            })
            .then(function (result) {
                self.savingDraft = false;
                
                // Update the local change request data
                currentChangeRequest.contentSnapshot = updatedContentSnapshot;
                
                piranha.notifications.push({
                    body: "Change request updated successfully",
                    type: "success"
                });
            })
            .catch(function (error) {
                self.savingDraft = false;
                console.log("Error updating change request:", error);
                piranha.notifications.push({
                    body: "Failed to update change request. Please try again.",
                    type: "danger"
                });
            });
        },        saveInternalWithReload: function (route) {
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
                self.saving = false;
                self.savingDraft = false;
                
                // Update the current page state
                self.id = result.id;
                self.slug = result.slug;
                self.published = result.published;
                self.publishedTime = result.publishedTime;
                self.state = result.state;
                self.isCopy = result.isCopy;
                self.selectedRoute = result.selectedRoute;
                
                // Show success message
                piranha.notifications.push(result.status);
                
                // Refresh the page data and change requests
                self.load(self.id);
            })
            .catch(function (error) {
                console.log("error:", error);
                self.saving = false;
                self.savingDraft = false;
                  // Show error message
                piranha.notifications.push({
                    body: "Failed to save page, but change request was created",
                    type: "warning"
                });
                
                // Still try to refresh the page data
                self.load(self.id);
            });
        }
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
