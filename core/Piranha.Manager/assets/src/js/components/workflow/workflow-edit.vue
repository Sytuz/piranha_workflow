<template>
  <div v-if="workflow">
    <div class="card">
      <div class="card-header">
        <span class="title">{{ isNew ? 'New Workflow' : workflow.title }}</span>
      </div>
      <div class="card-body">
        <div class="form-group">
          <label for="workflow-title">Name</label>
          <input id="workflow-title" v-model="workflow.title" class="form-control" :class="{ 'is-invalid': !isTitleValid }" @blur="validateTitle" />
          <div class="invalid-feedback" v-if="!isTitleValid">
            {{ titleError }}
          </div>
        </div>
        <div class="form-group">
          <label for="workflow-description">Description</label>
          <textarea id="workflow-description" v-model="workflow.description" class="form-control"></textarea>
        </div>
        <div class="form-group">
          <div class="form-check">
            <input id="workflow-default" v-model="workflow.isDefault" type="checkbox" class="form-check-input">
            <label for="workflow-default" class="form-check-label">Set as default workflow</label>
          </div>
        </div>
      </div>
    </div>

    <div class="card mt-4">
      <div class="card-header">
        <span class="title">Workflow Stages</span>
      </div>
      <div class="card-body">
        <div v-if="workflow.stages.length === 0" class="alert alert-info">
          No stages have been added yet
        </div>
        <div v-else>
          <div v-for="(stage, index) in workflow.stages" :key="stage.id" class="card mb-3">
            <div class="card-header">
              <span class="handle">
                <i class="fas fa-ellipsis-v"></i>
              </span>
              <span class="title">{{ stage.title || 'Untitled stage' }}</span>
              <div class="btn-group float-right">
                <button v-on:click="removeStage(index)" class="btn btn-sm btn-danger">
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </div>
            <div class="card-body">
              <div class="form-group">
                <label>Name</label>
                <input v-model="stage.title" class="form-control" required />
              </div>
              <div class="form-group">
                <label>Description</label>
                <textarea v-model="stage.description" class="form-control"></textarea>
              </div>
              <div class="form-group">
                <div class="form-check">
                  <input v-model="stage.isPublished" type="checkbox" class="form-check-input" :id="'stage-published-' + index">
                  <label class="form-check-label" :for="'stage-published-' + index">Content is published in this stage</label>
                </div>
              </div>
            </div>
          </div>
        </div>
        <button v-on:click="addStage" class="btn btn-primary">
          <i class="fas fa-plus"></i> Add Stage
        </button>
      </div>
    </div>

    <div class="card mt-4">
      <div class="card-body">
        <button v-on:click="save" class="btn btn-success" :disabled="!isTitleValid || !workflow.title">Save</button>
        <button v-on:click="cancel" class="btn btn-secondary ml-2">Cancel</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      loading: false,
      workflow: null,
      isTitleValid: true,
      titleError: ''
    }
  },
  computed: {
    isNew() {
      return !this.workflow.id || this.workflow.id === '00000000-0000-0000-0000-000000000000';
    }
  },
  methods: {
    load() {
      this.loading = true;

      const id = window.location.pathname.split('/').pop();

      if (id && id !== 'add') {
        fetch(piranha.baseUrl + 'manager/api/workflow/' + id)
          .then(response => response.json())
          .then(result => {
            this.workflow = result;
            this.loading = false;
          })
          .catch(error => {
            console.log("error:", error);
            this.loading = false;
          });
      } else {
        this.workflow = {
          id: '',
          title: '',
          description: '',
          isDefault: false,
          stages: []
        };
        this.loading = false;
      }
    },
    save() {
      if (!this.workflow.title) {
        this.isTitleValid = false;
        this.titleError = 'Name is required';
        return;
      }

      if (!this.isTitleValid) {
        return;
      }

      // Update sort order
      this.workflow.stages.forEach((stage, index) => {
        stage.sortOrder = index + 1;
      });

      fetch(piranha.baseUrl + 'manager/api/workflow/save', {
        method: 'POST',
        headers: piranha.utils.antiForgeryHeaders(),
        body: JSON.stringify(this.workflow)
      })
      .then(response => {
        if (!response.ok) {
          return response.json().then(error => { throw error; });
        }
        return response.json();
      })
      .then(result => {
        // Show notification
        piranha.notifications.push({
          body: 'The workflow was saved successfully',
          type: 'success'
        });

        // Go back to list
        window.location.href = piranha.baseUrl + 'manager/workflow';
      })
      .catch(error => {
        console.log("error:", error);
        piranha.notifications.push({
          body: error.body || 'An error occurred while saving the workflow',
          type: 'danger'
        });
      });
    },
    cancel() {
      window.location.href = piranha.baseUrl + 'manager/workflow';
    },
    addStage() {
      this.workflow.stages.push({
        id: '',
        workflowId: this.workflow.id,
        title: '',
        description: '',
        sortOrder: this.workflow.stages.length + 1,
        isPublished: false
      });
    },
    removeStage(index) {
      this.workflow.stages.splice(index, 1);
    },
    validateTitle() {
      if (!this.workflow.title) {
        this.isTitleValid = false;
        this.titleError = 'Name is required';
        return;
      }

      fetch(piranha.baseUrl + 'manager/api/workflow/title-unique?title=' + encodeURIComponent(this.workflow.title) + '&id=' + (this.workflow.id || ''))
        .then(response => response.json())
        .then(result => {
          this.isTitleValid = result;
          if (!result) {
            this.titleError = 'Name already in use';
          }
        })
        .catch(error => console.log("error:", error));
    }
  },
  created() {
    this.load();
  }
}
</script>
