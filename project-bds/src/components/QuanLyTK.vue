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
              <v-icon left class="mr-2">mdi-account-multiple</v-icon>
              <span>Quản Lý Người Dùng</span>
            </v-card-title>
          </v-card>
        </v-col>
      </v-card>

      <!-- Phần tìm kiếm -->
      <v-card class="pa-4 mb-4">
        <v-form>
          <v-row>
            <v-col cols="4">
              <v-text-field
                v-model="searchFullName"
                label="Tìm kiếm theo tên đầy đủ"
                solo
                outlined
                dense
                class="search-field"
                :disabled="loading"
                @keyup.enter="filterUsers"
              />
            </v-col>
            <v-col cols="4">
              <v-text-field
                v-model="searchEmail"
                label="Tìm kiếm theo email"
                solo
                outlined
                dense
                class="search-field"
                :disabled="loading"
                @keyup.enter="filterUsers"
              />
            </v-col>
            <v-col cols="4">
              <v-text-field
                v-model="searchPhone"
                label="Tìm kiếm theo số điện thoại"
                solo
                outlined
                dense
                class="search-field"
                :disabled="loading"
                @keyup.enter="filterUsers"
              />
            </v-col>
          </v-row>
        </v-form>
      </v-card>

      <v-card class="pa-4 mb-4">
        <v-simple-table class="full-width-table">
          <thead>
            <tr>
              <th class="text-left">Id</th>
              <th class="text-left">Tên người dùng</th>
              <th class="text-left">Email</th>
              <th class="text-left">Tên đầy đủ</th>
              <th class="text-left">Số điện thoại</th>
              <th class="text-left">Giới tính</th>
              <th class="text-left">Ngày sinh</th>
              <th class="text-left">Vai trò</th>
              <th class="text-left">Ngày tạo</th>
              <th class="text-left">Ngày cập nhật</th>
              <th class="text-left">Trạng thái</th>
              <th class="text-left">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="filteredUsers.length === 0">
              <td colspan="12" class="text-center">
                Không có người dùng nào được tìm thấy.
              </td>
            </tr>
            <tr v-else v-for="user in paginatedData" :key="user.id">
              <td>{{ user.id }}</td>
              <td>{{ user.userName }}</td>
              <td>{{ user.email }}</td>
              <td>{{ user.fullName }}</td>
              <td>{{ user.phoneNumber }}</td>
              <td>{{ getGenderName(user.gender) }}</td>
              <td>{{ new Date(user.dateOfBirth).toLocaleDateString() }}</td>
              <td>{{ getRoleName(user.roleId) }}</td>
              <td>{{ new Date(user.createTime).toLocaleDateString() }}</td>
              <td>{{ new Date(user.updateTime).toLocaleDateString() }}</td>
              <td>{{ getStatusName(user.statusId) }}</td>
              <td>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn
                      @click="confirmDelete(user.id)"
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
        </v-simple-table>

        <v-pagination
          v-model="currentPage"
          :length="totalPages"
          next-icon="mdi-menu-right"
          prev-icon="mdi-menu-left"
          @input="fetchUsers"
        ></v-pagination>
        <span>Trang {{ currentPage }} / {{ totalPages }}</span>
      </v-card>

      <!-- Dialog xác nhận xóa -->
      <v-dialog v-model="showConfirmDialog" max-width="290">
        <v-card>
          <v-card-title class="headline">Xác nhận</v-card-title>
          <v-card-text>
            Bạn có chắc chắn muốn xóa người dùng này không?
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="green darken-1" text @click="deleteUser">Có</v-btn>
            <v-btn color="red darken-1" text @click="cancelDelete">Không</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-col>
  </v-row>
</template>

<script>
import HomePage from "./HomePage.vue";
import axios from "axios";

export default {
  components: { HomePage },
  data() {
    return {
      loading: false,
      users: [],
      filteredUsers: [],
      currentPage: 1,
      perPage: 10,
      showConfirmDialog: false,
      userIdToDelete: null,
      searchFullName: "",
      searchEmail: "",
      searchPhone: "",
      roles: {
        1: "Admin",
        2: "Manager",
        3: "Staff",
        4: "User",
      },
    };
  },

  computed: {
    paginatedData() {
      const start = (this.currentPage - 1) * this.perPage;
      const end = start + this.perPage;
      return this.filteredUsers.slice(start, end);
    },
    totalPages() {
      return Math.ceil(this.filteredUsers.length / this.perPage);
    },
  },

  methods: {
    normalizeString(str) {
      return str
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .toLowerCase();
    },

    getStatusName(statusId) {
      switch (statusId) {
        case 1:
          return "Chưa kích hoạt";
        case 2:
          return "Đã kích hoạt";
        default:
          return "Chưa xác định";
      }
    },
    getGenderName(gender) {
      return gender ? "Nam" : "Nữ";
    },
    getRoleName(roleId) {
      return this.roles[roleId] || "Chưa xác định";
    },

    async fetchUsers() {
      this.loading = true;
      try {
        const token = localStorage.getItem("accessToken");
        if (!token) {
          alert("Token không có. Vui lòng đăng nhập lại.");
          this.$router.push("/login");
          return;
        }

        const response = await axios.get(
          `https://localhost:7067/api/User/GetAllUsers`,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        this.users = response.data.data;
        this.filteredUsers = this.users;
        this.totalPages = Math.ceil(this.filteredUsers.length / this.perPage);
      } catch (error) {
        console.error("Error fetching user details:", error);
        if (error.response && error.response.status === 401) {
          alert("Lỗi xác thực: Vui lòng đăng nhập lại.");
          localStorage.removeItem("accessToken");
          localStorage.removeItem("roleId");
          this.$router.push("/login");
        }
      } finally {
        this.loading = false;
      }
    },

    filterUsers() {
      this.currentPage = 1; // Reset trang về đầu khi tìm kiếm
      const fullName = this.normalizeString(this.searchFullName);
      const email = this.normalizeString(this.searchEmail);
      const phone = this.normalizeString(this.searchPhone);

      this.filteredUsers = this.users.filter((user) => {
        const userFullName = this.normalizeString(user.fullName);
        const userEmail = this.normalizeString(user.email);
        const userPhone = this.normalizeString(user.phoneNumber);

        return (
          (fullName === "" || userFullName.includes(fullName)) &&
          (email === "" || userEmail.includes(email)) &&
          (phone === "" || userPhone.includes(phone))
        );
      });

      this.totalPages = Math.ceil(this.filteredUsers.length / this.perPage);
    },

    async deleteUser() {
      if (this.userIdToDelete === null) return;

      try {
        const token = localStorage.getItem("accessToken");
        if (!token) {
          alert("Token không có. Vui lòng đăng nhập lại.");
          this.$router.push("/login");
          return;
        }

        await axios.delete(
          `https://localhost:7067/api/User/DeleteUser/${this.userIdToDelete}`,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        this.users = this.users.filter(
          (user) => user.id !== this.userIdToDelete
        );
        this.filterUsers(); // Cập nhật danh sách người dùng sau khi xóa
        this.userIdToDelete = null;
        this.showConfirmDialog = false;
      } catch (error) {
        console.error("Error deleting user:", error);
        if (error.response && error.response.status === 401) {
          alert("Lỗi xác thực: Vui lòng đăng nhập lại.");
          localStorage.removeItem("accessToken");
          localStorage.removeItem("roleId");
          this.$router.push("/login");
        }
      }
    },

    confirmDelete(userId) {
      this.userIdToDelete = userId;
      this.showConfirmDialog = true;
    },

    cancelDelete() {
      this.showConfirmDialog = false;
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
  border-collapse: collapse;
}

.full-width-table th,
.full-width-table td {
  border: 1px solid #e0e0e0;
  padding: 12px;
  text-align: left;
}

.full-width-table th {
  background-color: #f5f5f5;
  color: #333;
}

.full-width-table td {
  color: #555;
}

.pagination-container {
  margin-top: 20px;
}

.search-field {
  max-width: 300px;
}
</style>
