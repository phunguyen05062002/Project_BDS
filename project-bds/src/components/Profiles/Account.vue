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
              <v-icon left class="mr-2">mdi-account-circle</v-icon>
              <span>Thông tin tài khoản</span>
            </v-card-title>
          </v-card>
        </v-col>
      </v-card>

      <v-card class="pa-4 mb-4 shadow-sm">
        <form @submit.prevent="save">
          <v-row>
            <v-col cols="12" sm="6" class="mb-2">
              <v-text-field
                label="Tài khoản"
                v-model="userInfo.userName"
                outlined
                dense
                readonly
              ></v-text-field>
            </v-col>
            <v-col cols="12" sm="6" class="mb-2">
              <v-text-field
                label="Họ và tên"
                v-model="userInfo.fullName"
                :readonly="!isEditing"
                outlined
                dense
              ></v-text-field>
            </v-col>
            <v-col cols="12" sm="6" class="mb-2">
              <v-text-field
                label="Email"
                v-model="userInfo.email"
                :readonly="!isEditing"
                outlined
                dense
              ></v-text-field>
            </v-col>
            <v-col cols="12" sm="6" class="mb-2">
              <v-text-field
                label="Số điện thoại"
                v-model="userInfo.phoneNumber"
                :readonly="!isEditing"
                outlined
                dense
              ></v-text-field>
            </v-col>
            <v-col cols="12" sm="6" class="mb-2">
              <v-text-field
                label="Ngày sinh"
                v-model="formattedDateOfBirth"
                type="date"
                :readonly="!isEditing"
                outlined
                dense
              ></v-text-field>
            </v-col>
            <v-col cols="12" sm="6" class="mb-2">
              <v-select
                :items="genderItems"
                item-title="text"
                item-value="value"
                label="Giới tính"
                v-model="userInfo.gender"
                :disabled="!isEditing"
                outlined
                dense
                @change="handleGenderChange"
              ></v-select>
            </v-col>
            <v-col cols="12" class="d-flex justify-center">
              <v-alert v-if="errorMessage" type="error">{{
                errorMessage
              }}</v-alert>
              <v-alert v-if="successMessage" type="success">{{
                successMessage
              }}</v-alert>
            </v-col>
          </v-row>
          <v-row class="mt-4">
            <v-col cols="12" class="d-flex justify-center">
              <v-btn
                class="button-update me-2"
                :loading="loading"
                @click="toggleEdit"
                v-if="!isEditing"
              >
                Cập nhật
              </v-btn>
              <v-btn
                class="button-save me-2"
                :loading="loading"
                v-if="isEditing"
                type="submit"
              >
                Lưu lại
              </v-btn>
              <v-btn
                class="button-cancel me-2"
                @click="cancelEdit"
                v-if="isEditing"
              >
                Hủy bỏ
              </v-btn>
            </v-col>
            <v-col cols="12" class="d-flex justify-center">
              <v-btn class="button-logout" @click="logout">Đăng xuất</v-btn>
            </v-col>
          </v-row>
        </form>
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
import axios from "axios";
import HomePage from "../HomePage.vue";

export default {
  components: { HomePage },
  data() {
    return {
      userInfo: {
        userName: "",
        fullName: "",
        email: "",
        phoneNumber: "",
        dateOfBirth: "",
        gender: null, // Thay đổi từ boolean thành null để xử lý không có giá trị
      },
      genderItems: [
        { text: "Nam", value: true }, // true cho Nam
        { text: "Nữ", value: false }, // false cho Nữ
      ],
      isEditing: false,
      errorMessage: "",
      successMessage: "",
      loading: false,
    };
  },
  computed: {
    formattedDateOfBirth() {
      if (!this.userInfo.dateOfBirth) return "";
      const date = new Date(this.userInfo.dateOfBirth);
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, "0");
      const day = String(date.getDate()).padStart(2, "0");
      return `${year}-${month}-${day}`;
    },
    genderDisplay() {
      return this.userInfo.gender === null
        ? "Chưa chọn"
        : this.userInfo.gender
        ? "Nam"
        : "Nữ";
    },
  },
  methods: {
    async loadUserInfo() {
      try {
        const token = localStorage.getItem("accessToken");
        if (!token) {
          alert("Token không có. Vui lòng đăng nhập lại.");
          this.$router.push("/login");
          return;
        }
        const response = await axios.get(
          "https://localhost:7067/api/User/GetUserInfo",
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (
          response.data &&
          response.data.status === 200 &&
          response.data.data
        ) {
          // Cập nhật userInfo với dữ liệu từ API
          this.userInfo = {
            userName: response.data.data.userName || "",
            fullName: response.data.data.fullName || "",
            email: response.data.data.email || "",
            phoneNumber: response.data.data.phoneNumber || "",
            dateOfBirth: response.data.data.dateOfBirth || "",
            gender: response.data.data.gender, // Boolean từ API
          };
        } else {
          console.error("API Error Message:", response.data.message);
          alert(response.data.message);
        }
      } catch (error) {
        console.error("Error loading user info:", error);
        alert("Không thể tải thông tin người dùng. Vui lòng thử lại sau.");
      }
    },

    async save() {
      if (!this.isEditing) return;
      try {
        this.loading = true;
        const token = localStorage.getItem("accessToken");
        if (!token) {
          alert("Token không có. Vui lòng đăng nhập lại.");
          this.$router.push("/login");
          return;
        }
        const response = await axios.put(
          "https://localhost:7067/api/User/UpdateUserInfo",
          {
            userName: this.userInfo.userName,
            fullName: this.userInfo.fullName,
            email: this.userInfo.email,
            phoneNumber: this.userInfo.phoneNumber,
            dateOfBirth: this.userInfo.dateOfBirth,
            gender: this.userInfo.gender, // Boolean giá trị đúng
          },
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (response.data && response.data.status === 200) {
          this.successMessage = "Cập nhật thông tin thành công!";
          this.isEditing = false;
        } else {
          this.errorMessage = response.data.message || "Có lỗi xảy ra!";
        }
      } catch (error) {
        console.error("Error saving user info:", error);
        this.errorMessage = "Có lỗi xảy ra! Vui lòng thử lại sau.";
      } finally {
        this.loading = false;
      }
    },

    toggleEdit() {
      this.isEditing = !this.isEditing;
    },

    cancelEdit() {
      this.isEditing = false;
      this.loadUserInfo(); // Reload user info to reset changes
    },

    logout() {
      localStorage.removeItem("accessToken");
      this.$router.push("/login");
    },
  },
  created() {
    console.log("Initial userInfo.gender:", this.userInfo.gender); // Log giá trị khởi tạo
    this.loadUserInfo();
  },
  handleGenderChange(value) {
    console.log("Gender selected:", value); // Log giá trị mới của gender
    this.userInfo.gender = value;
  },
};
</script>

<style scoped>
.button-update {
  background-color: #007bff;
  color: white;
}

.button-update:hover {
  background-color: #0069d9;
}

.button-save {
  background-color: #28a745;
  color: white;
}

.button-save:hover {
  background-color: #218838;
}

.button-cancel {
  background-color: #ffc107;
  color: white;
}

.button-cancel:hover {
  background-color: #e0a800;
}

.button-logout {
  background-color: #dc3545;
  color: white;
}

.button-logout:hover {
  background-color: #c82333;
}
</style>
