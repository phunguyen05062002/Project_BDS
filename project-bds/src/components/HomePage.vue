<template>
  <v-layout>
    <v-navigation-drawer permanent>
      <v-row>
        <v-col cols="3">
          <img src="../../public/icon.jpg" alt="Logo" width="50" height="50" />
        </v-col>
        <v-col cols="9">
          <h4 class="pa-4">BĐS Tuấn Anh</h4>
        </v-col>
      </v-row>
      <v-divider></v-divider>
      <v-list density="compact" nav>
        <v-list-item
          v-if="shouldShowMenu('home')"
          prepend-icon="mdi-home"
          title="Trang chủ"
          value="home"
          to="/trang-chu"
        ></v-list-item>

        <v-list-item
          v-if="shouldShowMenu('phanQuyen')"
          prepend-icon="mdi-shield-account"
          title="Phân quyền"
          @click="toggleDecentralizationMenu"
          class="decentralization-menu"
        ></v-list-item>

        <div
          v-if="isDecentralizationMenuOpen && shouldShowMenu('phanQuyen')"
          class="nested-menu"
        >
          <v-list-item
            prepend-icon="mdi-shield-account"
            title="Quản lý quyền"
            to="/phan-quyen/quan-ly-quyen"
            class="nested-item"
          ></v-list-item>

          <v-list-item
            prepend-icon="mdi-account-key"
            title="Cập nhật quyền"
            to="/phan-quyen/cap-nhat-quyen"
            class="nested-item"
          ></v-list-item>
        </div>

        <v-list-item
          v-if="shouldShowMenu('qlBDS')"
          prepend-icon="mdi-city"
          title="Quản lý bất động sản"
          value="products"
          @click="toggleqlBDSMenu"
          class="qlbds-menu"
        ></v-list-item>

        <div
          v-if="isqlBDSMenuOpen && shouldShowMenu('qlBDS')"
          class="nested-menu"
        >
          <v-list-item
            prepend-icon="mdi-database"
            title="Quản lý thông tin"
            to="/quan-ly-BDS/quan-ly-thong-tin"
            class="nested-item"
          ></v-list-item>

          <v-list-item
            prepend-icon="mdi-home-city"
            title="Quản lý loại BĐS"
            to="/quan-ly-BDS/quan-ly-loai-BDS"
            class="nested-item"
          ></v-list-item>

          <v-list-item
            prepend-icon="mdi-image"
            title="Quản lý ảnh"
            to="/quan-ly-BDS/quan-ly-anh"
            class="nested-item"
          ></v-list-item>
        </div>

        <v-list-item
          v-if="shouldShowMenu('qlTK')"
          prepend-icon="mdi-account-multiple"
          title="Quản lý tài khoản"
          value="users"
          to="/quan-ly-Tk"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-account"
          title="Cá nhân"
          @click="toggleProfileMenu"
          class="profile-menu"
        ></v-list-item>

        <div v-if="isProfileMenuOpen" class="nested-menu">
          <v-list-item
            prepend-icon="mdi-account-circle"
            title="Tài khoản"
            to="/ca-nhan/tai-khoan"
            class="nested-item"
          ></v-list-item>

          <v-list-item
            prepend-icon="mdi-key"
            title="Đổi mật khẩu"
            to="/ca-nhan/doi-mat-khau"
            class="nested-item"
          ></v-list-item>
        </div>

        <v-list-item
          v-if="shouldShowMenu('thongKe')"
          prepend-icon="mdi-chart-bar"
          title="Thống kê"
          value="statistics"
          to="/thong-ke"
        ></v-list-item>

        <v-list-item
          v-if="shouldShowMenu('lienHe')"
          prepend-icon="mdi-message-outline"
          title="Liên hệ"
          value="contact"
          to="/lien-he"
        ></v-list-item>

        <v-list-item
          v-if="shouldShowMenu('guiLichHen')"
          prepend-icon="mdi-calendar-clock"
          title="Gửi lịch hẹn"
          value="sendAppointment"
          to="/gui-lich-hen"
        ></v-list-item>
      </v-list>
    </v-navigation-drawer>
    <v-row>
      <v-col cols="2">
        <v-main style="height: 100vh"></v-main>
      </v-col>
      <v-col cols="10">
        <router-view></router-view>
      </v-col>
    </v-row>
  </v-layout>
</template>

<script>
import Roles from "../Helper/Roles.js";

export default {
  name: "App",
  data() {
    return {
      loading: false,
      selection: 1,
      tab: "trang-chu",
      drawer: true,
      rail: true,
      dateRangeMenu: false,
      dateRange: {
        from: null,
        to: null,
      },
      isProfileMenuOpen: false,
      isDecentralizationMenuOpen: false,
      isqlBDSMenuOpen: false,
      userRoleId: null,
    };
  },
  created() {
    this.setUserRole();
  },
  methods: {
    reserve() {
      this.loading = true;

      setTimeout(() => (this.loading = false), 2000);
    },
    toggleProfileMenu() {
      this.isProfileMenuOpen = !this.isProfileMenuOpen;
    },
    toggleDecentralizationMenu() {
      this.isDecentralizationMenuOpen = !this.isDecentralizationMenuOpen;
    },
    toggleqlBDSMenu() {
      this.isqlBDSMenuOpen = !this.isqlBDSMenuOpen;
    },
    setUserRole() {
      // Giả sử token đã được lưu trữ trong localStorage và chứa thông tin RoleId
      const token = localStorage.getItem("accessToken");
      if (token) {
        const payload = JSON.parse(atob(token.split(".")[1]));
        this.userRoleId = parseInt(payload.RoleId);
      }
    },
    shouldShowMenu(menu) {
      // Kiểm tra xem menu có nên hiển thị dựa trên RoleId
      switch (menu) {
        case "home":
          return true; // Trang chủ luôn hiển thị
        case "phanQuyen":
          return this.userRoleId === Roles.Admin;
        case "qlBDS":
          return this.userRoleId === Roles.Manager;
        case "qlTK":
          return this.userRoleId === Roles.Admin;
        case "thongKe":
          return (
            this.userRoleId === Roles.Admin ||
            this.userRoleId === Roles.Manager ||
            this.userRoleId === Roles.Staff ||
            this.userRoleId === Roles.User
          );
        case "lienHe":
          return this.userRoleId === Roles.User;
        case "guiLichHen":
          return this.userRoleId === Roles.Staff;
        default:
          return false;
      }
    },
  },
};
</script>

<style scoped>
.header {
  margin-bottom: 20px;
  background-color: #fff;
  border-bottom: 2px solid #e0e0e0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.sidebar {
  background-color: #f5f5f5;
  padding: 20px;
  height: calc(100vh - 64px);
  overflow-y: auto;
  margin-top: 20px;
}

.content {
  padding: 20px;
}

.v-card {
  margin-top: 20px;
}

.main-container {
  margin-top: 20px;
  margin-bottom: 20px;
}

.v-footer {
  background-color: #f5f5f5;
  padding: 10px 0;
}

.v-list-item {
  cursor: pointer;
}

.v-list-item:hover {
  background-color: #e0e0e0;
}

.profile-menu .decentralization-menu .qlbds-menu {
  background-color: #e3f2fd;
}

.nested-menu {
  padding-left: 16px;
}

.nested-item {
  font-size: 0.875rem;
  background-color: #f5f5f5;
}

.nested-item:hover {
  background-color: #e0e0e0;
}
</style>
