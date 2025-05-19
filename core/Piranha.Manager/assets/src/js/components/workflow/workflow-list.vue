<template>
  <div class="card">
    <div class="card-header">
      <span class="title">Workflows</span>
      <div class="btn-group float-right">
        <button v-on:click="addWorkflow" class="btn btn-primary btn-labeled">
          <i class="fas fa-plus"></i>New workflow
        </button>
      </div>
    </div>
    <div class="card-body">
      <table v-if="items.length > 0" class="table table-borderless">
        <thead>
          <tr>
            <th>Name</th>
            <th>Stages</th>
            <th>Default</th>
            <th>Created</th>
            <th>Last Modified</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="workflow in items" :key="workflow.id">
            <td>{{ workflow.title }}</td>
            <td>{{ workflow.stages.length }}</td>
            <td>
              <i v-if="workflow.isDefault" class="fas fa-check text-success"></i>
            </td>
            <td>{{ formatDate(workflow.created) }}</td>
            <td>{{ formatDate(workflow.lastModified) }}</td>
            <td class="actions">
              <button v-on:click="editWorkflow(workflow)" class="btn btn-sm btn-primary">
                <i class="fas fa-pen"></i>
              </button>
              <button v-on:click="removeWorkflow(workflow)" class="btn btn-sm btn-danger">
                <i class="fas fa-trash"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
      <div v-else class="empty-info">
        <div class="card text-center">
          <div class="card-body">
            <h3>No workflows have been added yet</h3>
            <p>Click on the button above to add a new workflow</p>
          </div>
        </div>
      </div>
    </div>

    <div class="modal fade" id="workflowModal" tabindex="-1" role="dialog" aria-labelledby="workflowModalLabel" aria-hidden="true">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="workflowModalLabel">New Workflow</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <div class="form-group">
              <label for="workflow-title">Name</label>
              <input id="workflow-title" v-model="newWorkflow.title" class="form-control" :class="{ 'is-invalid': !isTitleValid }" @blur="validateTitle" />
              <div class="invalid-feedback" v-if="!isTitleValid">
                {{ titleError }}
              </div>
            </div>
            <div class="form-group">
              <label for="workflow-description">Description</label>
              <textarea id="workflow-description" v-model="newWorkflow.description" class="form-control"></textarea>
            </div>
            <div class="form-group">
              <label>Workflow Type</label>
              <div class="form-check">
                <input class="form-check-input" type="radio" name="workflowType" id="workflow-type-blank" value="blank" v-model="workflowType">
                <label class="form-check-label" for="workflow-type-blank">
                  Blank Workflow
                </label>
              </div>
              <div class="form-check">
                <input class="form-check-input" type="radio" name="workflowType" id="workflow-type-standard" value="standard" v-model="workflowType">
                <label class="form-check-label" for="workflow-type-standard">
                  Standard Workflow (Draft → Review → Approved)
                </label>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
            <button type="button" class="btn btn-primary" v-on:click="createWorkflow" :disabled="!isTitleValid || !newWorkflow.title">Create</button>
          </div>
        </div>
      </div>
    </div>

    <div class="modal fade" id="workflowDeleteModal" tabindex="-1" role="dialog" aria-labelledby="workflowDeleteModalLabel" aria-hidden="true">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="workflowDeleteModalLabel">Delete Workflow</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            Are you sure you want to delete the workflow "{{ selectedWorkflow.title }}"?
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
            <button type="button" class="btn btn-danger" @click="confirmDeleteWorkflow">Delete</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      items: [],
      newWorkflow: {
        title: '',
        description: ''
      },
      workflowType: 'standard',
      isTitleValid: true,
      titleError: 'Name is required',
      selectedWorkflow: {
        id: '',
        title: ''
      }
    }
  },
  methods: {
    load() {
      fetch(piranha.baseUrl + 'manager/api/workflow/list')
        .then(response => response.json())
        .then(result => {
          this.items = result;
        })
        .catch(error => console.log("error:", error));
    },
    addWorkflow() {
      this.newWorkflow = {
        title: '',
        description: ''
      };
      this.isTitleValid = true;
      this.titleError = '';
      this.workflowType = 'standard';

      $('#workflowModal').modal('show');
    },
    validateTitle() {
      if (!this.newWorkflow.title) {
        this.isTitleValid = false;
        this.titleError = 'Name is required';
        return;
      }

      fetch(piranha.baseUrl + 'manager/api/workflow/title-unique?title=' + encodeURIComponent(this.newWorkflow.title))
        .then(response => response.json())
        .then(result => {
          this.isTitleValid = result;
          if (!result) {
            this.titleError = 'Name already in use';
          }
        })
        .catch(error => console.log("error:", error));
    },
    createWorkflow() {
      if (!this.newWorkflow.title) {
        this.isTitleValid = false;
        this.titleError = 'Name is required';
        return;
      }

      if (!this.isTitleValid) {
        return;
      }

      if (this.workflowType === 'standard') {
        fetch(piranha.baseUrl + 'manager/api/workflow/create-standard', {
          method: 'POST',
          headers: piranha.utils.antiForgeryHeaders(),
          body: JSON.stringify(this.newWorkflow)
        })
        .then(response => {
          if (!response.ok) {
            return response.json().then(error => { throw error; });
          }
          return response.json();
        })
        .then(result => {
          this.load();
          $('#workflowModal').modal('hide');

          // Show notification
          piranha.notifications.push({
            body: 'The workflow was created successfully',
            type: 'success'
          });
        })
        .catch(error => {
          console.log("error:", error);
          piranha.notifications.push({
            body: error.body || 'An error occurred while creating the workflow',
            type: 'danger'
          });
        });
      } else {
        fetch(piranha.baseUrl + 'manager/api/workflow/save', {
          method: 'POST',
          headers: piranha.utils.antiForgeryHeaders(),
          body: JSON.stringify({
            id: '',
            title: this.newWorkflow.title,
            description: this.newWorkflow.description,
            isDefault: false,
            stages: []
          })
        })
        .then(response => {
          if (!response.ok) {
            return response.json().then(error => { throw error; });
          }
          return response.json();
        })
        .then(result => {
          this.load();
          $('#workflowModal').modal('hide');

          // Show notification
          piranha.notifications.push({
            body: 'The workflow was created successfully',
            type: 'success'
          });
        })
        .catch(error => {
          console.log("error:", error);
          piranha.notifications.push({
            body: error.body || 'An error occurred while creating the workflow',
            type: 'danger'
          });
        });
      }
    },
    editWorkflow(workflow) {
      window.location.href = piranha.baseUrl + 'manager/workflow/edit/' + workflow.id;
    },
    removeWorkflow(workflow) {
      this.selectedWorkflow = workflow;
      $('#workflowDeleteModal').modal('show');
    },
    confirmDeleteWorkflow() {
      fetch(piranha.baseUrl + 'manager/api/workflow/' + this.selectedWorkflow.id, {
        method: 'DELETE',
        headers: piranha.utils.antiForgeryHeaders()
      })
      .then(response => {
        $('#workflowDeleteModal').modal('hide');

        // Remove from list
        this.items = this.items.filter(w => w.id !== this.selectedWorkflow.id);

        // Show notification
        piranha.notifications.push({
          body: 'The workflow was deleted successfully',
          type: 'success'
        });
      })
      .catch(error => {
        console.log("error:", error);
        piranha.notifications.push({
          body: 'An error occurred while deleting the workflow',
          type: 'danger'
        });
      });
    },
    formatDate(date) {
      return new Date(date).toLocaleDateString();
    }
  },
  created() {
    this.load();
  }
}
</script>
