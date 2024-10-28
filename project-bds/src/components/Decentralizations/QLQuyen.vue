<template>
  <v-row>
    <v-col cols="2">
      <home-page />
    </v-col>

    <v-col cols="10" class="content">
      <v-card class="pa-4 mb-4">
        <v-col cols="12" class="text-center mb-4">
          <v-card class="title-card mx-auto" max-width="600">
            <v-card-title>
              <v-icon left class="mr-2">mdi-account-cog</v-icon>
              <span>Quản Lý Quyền</span>
            </v-card-title>
          </v-card>
        </v-col>
      </v-card>
      <v-card class="pa-4 mb-4">
        <!-- Form thêm mới vai trò -->
        <v-form ref="roleForm" v-model="valid" lazy-validation>
          <v-text-field
            v-model="newRole.roleCode"
            label="Role Code"
            :rules="[rules.required]"
          ></v-text-field>
          <v-text-field
            v-model="newRole.roleName"
            label="Role Name"
            :rules="[rules.required]"
          ></v-text-field>
          <v-btn @click="addRole" :loading="loading" color="primary"
            >Thêm Vai Trò</v-btn
          >
          <v-btn @click="updateRole" :loading="loading" color="success"
            >Cập Nhật Vai Trò</v-btn
          >
        </v-form>
      </v-card>

      <v-card class="pa-4 mb-4">
        <v-table class="full-width-table">
          <thead>
            <tr>
              <th class="text-left">Id</th>
              <th class="text-left">RoleCode</th>
              <th class="text-left">RoleName</th>
              <th class="text-left">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="role in roles" :key="role.id">
              <td>{{ role.id }}</td>
              <td>{{ role.roleCode }}</td>
              <td>{{ role.roleName }}</td>
              <td>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn
                      @click="editRole(role)"
                      icon
                      v-bind="attrs"
                      v-on="on"
                    >
                      <v-icon>mdi-pencil</v-icon>
                    </v-btn>
                  </template>
                  <span>Sửa</span>
                </v-tooltip>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn
                      @click="deleteRole(role.id)"
                      icon
                      v-bind="attrs"
                      v-on="on"
                    >
                      <v-icon>mdi-delete</v-icon>
                    </v-btn>
                  </template>
                  <span>Xóa</span>
                </v-tooltip>
              </td>
            </tr>
          </tbody>
        </v-table>
      </v-card>

      <v-card
        :disabled="loading"
        :loading="loading"
        class="mx-auto my-12"
        max-width="374"
      >
        <template v-slot:loader="{ isActive }">
          <v-progress-linear
            :active="isActive"
            color="deep-purple"
            height="4"
            indeterminate
          ></v-progress-linear>
        </template>
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
import HomePage from "../HomePage.vue";
import axios from "axios";

export default {
  components: { HomePage },
  data() {
    return {
      loading: false,
      valid: false,
      roles: [],
      newRole: {
        id: null,
        roleCode: "",
        roleName: "",
      },
      rules: {
        required: (value) => !!value || "Trường này là bắt buộc",
      },
    };
  },
  methods: {
    async fetchRoles() {
      this.loading = true;
      try {
        const token = localStorage.getItem("accessToken");
        if (!token) throw new Error("Token không có hoặc không hợp lệ.");

        const response = await axios.get(
          "https://localhost:7067/api/Role/GetAllRoles",
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        if (response.status === 200) {
          this.roles = response.data.data; // Điều chỉnh nếu cấu trúc phản hồi khác
        }
      } catch (error) {
        if (error.response && error.response.status === 401) {
          alert("Lỗi xác thực: Vui lòng đăng nhập lại.");
          localStorage.removeItem("accessToken");
          localStorage.removeItem("refreshToken");
          this.$router.push("/login");
        } else {
          alert("Lỗi khi lấy danh sách vai trò: " + error.message);
        }
      } finally {
        this.loading = false;
      }
    },
    async addRole() {
      this.loading = true;
      try {
        const token = localStorage.getItem("accessToken");
        if (!token) throw new Error("Token không có hoặc không hợp lệ.");

        const response = await axios.post(
          "https://localhost:7067/api/Role/CreateRole",
          this.newRole,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        if (response.status === 201) {
          alert("Vai trò đã được thêm thành công.");
          this.fetchRoles(); // Tải lại danh sách vai trò
          this.resetForm(); // Reset form sau khi thêm thành công
        }
      } catch (error) {
        alert("Lỗi khi thêm vai trò: " + error.message);
      } finally {
        this.loading = false;
      }
    },
    async updateRole() {
      this.loading = true;
      try {
        if (!this.newRole.id)
          throw new Error("Không có ID vai trò để cập nhật.");

        const token = localStorage.getItem("accessToken");
        if (!token) throw new Error("Token không có hoặc không hợp lệ.");

        const response = await axios.put(
          `https://localhost:7067/api/Role/UpdateRole/${this.newRole.id}`,
          this.newRole,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        if (response.status === 200) {
          alert("Vai trò đã được cập nhật thành công.");
          this.fetchRoles(); // Tải lại danh sách vai trò
          this.resetForm(); // Reset form sau khi cập nhật thành công
        }
      } catch (error) {
        alert("Lỗi khi cập nhật vai trò: " + error.message);
      } finally {
        this.loading = false;
      }
    },
    async deleteRole(id) {
      this.loading = true;
      try {
        const token = localStorage.getItem("accessToken");
        if (!token) throw new Error("Token không có hoặc không hợp lệ.");

        const response = await axios.delete(
          `https://localhost:7067/api/Role/DeleteRole/${id}`,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        if (response.status === 200) {
          alert("Vai trò đã được xóa thành công.");
          this.fetchRoles(); // Tải lại danh sách vai trò
        }
      } catch (error) {
        alert("Lỗi khi xóa vai trò: " + error.message);
      } finally {
        this.loading = false;
      }
    },
    editRole(role) {
      this.newRole = { ...role }; // Sao chép vai trò vào form sửa
    },
    resetForm() {
      this.newRole = { id: null, roleCode: "", roleName: "" }; // Reset form
      this.$refs.roleForm.reset(); // Đặt lại trạng thái của form
    },
  },
  mounted() {
    this.fetchRoles(); // Tải danh sách vai trò khi component được mount
  },
};
</script>

<style scoped>
.full-width-table {
  width: 100%;
  table-layout: fixed;
}

.full-width-table thead th,
.full-width-table tbody td {
  padding: 8px;
}

.full-width-table th {
  text-align: left;
  border-right: 1px solid #e0e0e0;
}

.full-width-table tbody td {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  border-right: 1px solid #e0e0e0;
}

.full-width-table tbody tr:last-child td {
  border-bottom: 1px solid #e0e0e0;
}

.full-width-table thead tr th:last-child,
.full-width-table tbody tr td:last-child {
  border-right: none;
}

.v-btn {
  margin-right: 8px;
}
</style>
