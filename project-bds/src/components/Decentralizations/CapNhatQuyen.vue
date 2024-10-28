<template>
  <div>
    <v-row>
      <v-col cols="2">
        <home-page />
      </v-col>

      <v-col cols="10" class="content">
        <v-card class="pa-4 mb-4">
          <v-col cols="12" class="text-center mb-4">
            <v-card class="title-card mx-auto" max-width="600">
              <v-card-title>
                <v-icon left class="mr-2">mdi-shield-sync</v-icon>
                <span>Cập nhật quyền</span>
              </v-card-title>
            </v-card>
          </v-col>
        </v-card>

        <v-card class="pa-4 mb-4">
          <v-table class="full-width-table">
            <thead>
              <tr>
                <th class="text-left">Id</th>
                <th class="text-left">UserName</th>
                <th class="text-left">Email</th>
                <th class="text-left">RoleName</th>
                <th class="text-left">Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="user in paginatedData" :key="user.id">
                <td>{{ user.id }}</td>
                <td>{{ user.userName }}</td>
                <td>{{ user.email }}</td>
                <td>
                  <span v-if="!user.isEditingRole">
                    {{ getRoleName(user.roleId) }}
                  </span>
                  <v-select
                    v-else
                    :items="items"
                    v-model="user.updatedRoleId"
                    item-title="text"
                    item-value="value"
                    label="Chọn vai trò"
                  ></v-select>
                </td>
                <td>
                  <v-btn
                    @click="editUserRole(user)"
                    v-if="!user.isEditingRole"
                    color="primary"
                    >Cập nhật</v-btn
                  >
                  <v-btn @click="saveUserRole(user)" v-else color="success"
                    >Lưu</v-btn
                  >
                  <v-btn
                    @click="cancelEdit(user)"
                    v-if="user.isEditingRole"
                    color="grey"
                    >Hủy</v-btn
                  >
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

        <v-card class="pa-4 mb-4">
          <div class="text-center">
            <v-pagination
              v-model="currentPage"
              :length="totalPages"
              next-icon="mdi-menu-right"
              prev-icon="mdi-menu-left"
              @input="fetchUsers"
            ></v-pagination>
            <span>Trang {{ currentPage }} / {{ totalPages }}</span>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import axios from "axios";
import HomePage from "../HomePage.vue";

export default {
  components: { HomePage },
  data() {
    return {
      loading: false,
      users: [],
      items: [
        { text: "Admin", value: 1 },
        { text: "Manager", value: 2 },
        { text: "Staff", value: 3 },
        { text: "User", value: 4 },
      ],
      currentPage: 1,
      perPage: 10,
    };
  },
  computed: {
    paginatedData() {
      const start = (this.currentPage - 1) * this.perPage;
      const end = start + this.perPage;
      return this.users.slice(start, end);
    },
    totalPages() {
      return Math.ceil(this.users.length / this.perPage);
    },
  },
  methods: {
    async fetchUsers() {
      try {
        this.loading = true;
        const token = localStorage.getItem("accessToken");
        if (!token) {
          alert("Token không có. Vui lòng đăng nhập lại.");
          this.$router.push("/login");
          return;
        }
        const response = await axios.get(
          `https://localhost:7067/api/User/GetAllUserDetails?page=${this.currentPage}&size=${this.perPage}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        this.users = response.data.data.map((user) => ({
          ...user,
          updatedRoleId: user.roleId,
          isEditingRole: false,
        }));
      } catch (error) {
        console.error("Error fetching user details:", error);
      } finally {
        this.loading = false;
      }
    },
    editUserRole(user) {
      user.isEditingRole = true;
    },
    async saveUserRole(user) {
      try {
        this.loading = true;
        const token = localStorage.getItem("accessToken");
        if (!token) {
          alert("Token không có. Vui lòng đăng nhập lại.");
          this.$router.push("/login");
          return;
        }
        const response = await axios.put(
          `https://localhost:7067/api/User/UpdateRoleUserRoleName/updateUserRole`,
          {
            userId: user.id,
            newRoleId: user.updatedRoleId,
          },
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        console.log("Role updated successfully:", response.data.message);
        await this.fetchUsers();
      } catch (error) {
        console.error("Error updating user role:", error);
      } finally {
        this.loading = false;
      }
      user.isEditingRole = false;
    },
    cancelEdit(user) {
      user.isEditingRole = false;
    },
    getRoleName(roleId) {
      const role = this.items.find((item) => item.value === roleId);
      return role ? role.text : "Unknown";
    },
  },
  mounted() {
    this.fetchUsers();
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
