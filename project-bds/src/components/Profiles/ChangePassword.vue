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
              <v-icon left class="mr-2">mdi-lock-reset</v-icon>
              <span>Đổi mật khẩu</span>
            </v-card-title>
          </v-card>
        </v-col>
      </v-card>
      <v-card class="pa-4 mb-4">
        <form @submit.prevent="submit">
          <v-row>
            <v-col cols="6" class="mb-2">
              <v-text-field
                v-model="passwordOld"
                :append-icon="showPasswordOld ? 'mdi-eye' : 'mdi-eye-off'"
                :type="showPasswordOld ? 'text' : 'password'"
                @click:append="showPasswordOld = !showPasswordOld"
                :rules="passwordOldRules"
                label="Mật khẩu cũ"
                required
              ></v-text-field>
            </v-col>
            <v-col cols="6" class="mb-2"></v-col>
            <v-col cols="6" class="mb-2">
              <v-text-field
                v-model="passwordNew"
                :append-icon="showPasswordNew ? 'mdi-eye' : 'mdi-eye-off'"
                :type="showPasswordNew ? 'text' : 'password'"
                @click:append="showPasswordNew = !showPasswordNew"
                :rules="passwordNewRules"
                label="Mật khẩu mới"
                required
              ></v-text-field>
            </v-col>
            <v-col cols="6" class="mb-2">
              <v-text-field
                v-model="confirmPassword"
                :append-icon="showConfirmPassword ? 'mdi-eye' : 'mdi-eye-off'"
                :type="showConfirmPassword ? 'text' : 'password'"
                @click:append="showConfirmPassword = !showConfirmPassword"
                :rules="confirmPasswordRules"
                label="Xác nhận mật khẩu"
                required
              ></v-text-field>
            </v-col>
            <v-col cols="12" v-if="errorMessage">
              <v-alert type="error">{{ errorMessage }}</v-alert>
            </v-col>
            <v-col cols="12" v-if="successMessage">
              <v-alert type="success">{{ successMessage }}</v-alert>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="3" class="mb-2">
              <v-btn
                class="button-update me-4"
                :loading="loading"
                type="submit"
              >
                Cập nhật
              </v-btn>
              <v-btn class="button-reset" @click="resetForm"> Làm mới </v-btn>
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
      passwordOld: "",
      passwordNew: "",
      confirmPassword: "",
      showPasswordOld: false,
      showPasswordNew: false,
      showConfirmPassword: false,
      loading: false,
      errorMessage: "",
      successMessage: "",
      passwordOldRules: [(v) => !!v || "Mật khẩu cũ là bắt buộc"],
      passwordNewRules: [
        (v) => !!v || "Mật khẩu mới là bắt buộc",
        (v) =>
          /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/.test(
            v
          ) ||
          "Mật khẩu phải có ít nhất 8 ký tự, ít nhất 1 ký tự in hoa, ít nhất 1 ký tự in thường, ít nhất 1 số và 1 ký tự đặc biệt!",
      ],
      confirmPasswordRules: [
        (v) => !!v || "Xác nhận mật khẩu là bắt buộc",
        (v) =>
          v === this.passwordNew ||
          "Xác nhận mật khẩu không khớp với mật khẩu mới",
      ],
    };
  },
  methods: {
    async submit() {
      if (this.passwordNew !== this.confirmPassword) {
        this.errorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
        return;
      }

      this.loading = true;
      this.errorMessage = "";
      this.successMessage = "";

      try {
        const token = localStorage.getItem("accessToken");
        const response = await axios.put(
          "https://localhost:7067/api/Auth/ChangePassword",
          {
            OldPassword: this.passwordOld,
            NewPassword: this.passwordNew,
            ConfirmPassword: this.confirmPassword,
          },
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (response.status === 200) {
          this.successMessage = "Đổi mật khẩu thành công!";
        } else {
          this.errorMessage =
            response.data.message || "Đã xảy ra lỗi khi đổi mật khẩu.";
        }
      } catch (error) {
        this.errorMessage =
          error.response?.data?.message || "Đã xảy ra lỗi khi đổi mật khẩu.";
      } finally {
        this.loading = false;
      }
    },
    resetForm() {
      this.passwordOld = "";
      this.passwordNew = "";
      this.confirmPassword = "";
      this.errorMessage = "";
      this.successMessage = "";
      this.showPasswordOld = false;
      this.showPasswordNew = false;
      this.showConfirmPassword = false;
    },
  },
};
</script>

<style scoped>
.content {
  background-color: #f5f5f5;
  padding: 16px;
}

.v-btn {
  font-weight: 600;
}

.button-update {
  background-color: #007bff;
  color: white;
}

.button-update:hover {
  background-color: #0056b3;
}

.button-reset {
  background-color: #6c757d;
  color: white;
}

.button-reset:hover {
  background-color: #5a6268;
}
</style>
