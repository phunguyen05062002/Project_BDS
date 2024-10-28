<template>
  <v-container>
    <v-form ref="resetPasswordForm" @submit.prevent="submit">
      <v-row no-gutters class="auth-wrapper bg-surface">
        <v-col
          cols="12"
          lg="6"
          class="auth-card-v2 d-flex align-center justify-center mx-auto"
        >
          <v-card flat class="mt-12 mt-sm-0 pa-4" max-width="500">
            <v-card-text class="text-center">
              <v-img
                src="../public/icon.jpg"
                :width="100"
                class="mb-6"
                alt="Logo"
              ></v-img>
              <h5 class="text-h5 mb-1">Đặt lại mật khẩu</h5>
              <p class="mb-0">
                Nhập mã đặt lại mật khẩu và mật khẩu mới của bạn.
              </p>
            </v-card-text>

            <v-card-text>
              <v-row>
                <!-- Reset Code -->
                <v-col cols="12">
                  <v-text-field
                    v-model="resetCode"
                    :rules="resetCodeRules"
                    label="Mã đặt lại mật khẩu"
                    required
                  ></v-text-field>
                </v-col>

                <!-- New Password -->
                <v-col cols="12">
                  <v-text-field
                    v-model="newPassword"
                    :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                    :type="showPassword ? 'text' : 'password'"
                    @click:append="showPassword = !showPassword"
                    :rules="passwordRules"
                    label="Mật khẩu mới"
                    required
                  ></v-text-field>
                </v-col>

                <!-- Confirm Password -->
                <v-col cols="12">
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

                <!-- Action -->
                <v-col cols="12">
                  <v-row>
                    <!-- Button "Đặt lại mật khẩu" -->
                    <v-col cols="6">
                      <v-btn
                        block
                        :loading="loading"
                        class="button-login"
                        type="submit"
                      >
                        Đặt lại mật khẩu
                      </v-btn>
                    </v-col>
                    <!-- Button "Trở về trang đăng nhập" -->
                    <v-col cols="6">
                      <v-btn block color="primary" @click="goToLogin">
                        Trở về trang đăng nhập
                      </v-btn>
                    </v-col>
                  </v-row>
                </v-col>

                <!-- Error Message -->
                <v-col cols="12" v-if="errorMessage">
                  <v-alert type="error">{{ errorMessage }}</v-alert>
                </v-col>

                <!-- Success Message -->
                <v-col cols="12" v-if="successMessage">
                  <v-alert type="success">{{ successMessage }}</v-alert>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-form>
  </v-container>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      resetCode: "",
      newPassword: "",
      confirmPassword: "",
      showPassword: false,
      showConfirmPassword: false,
      loading: false,
      errorMessage: "",
      successMessage: "",
      resetCodeRules: [(v) => !!v || "Mã đặt lại mật khẩu là bắt buộc"],
      passwordRules: [
        (v) => !!v || "Mật khẩu là bắt buộc",
        (v) =>
          /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/.test(
            v
          ) ||
          "Mật khẩu phải có ít nhất 8 ký tự, ít nhất 1 ký tự in hoa, ít nhất 1 ký tự in thường, ít nhất 1 số và 1 ký tự đặc biệt!",
      ],
      confirmPasswordRules: [
        (v) => !!v || "Xác nhận mật khẩu là bắt buộc",
        (v) =>
          v === this.newPassword || "Mật khẩu và xác nhận mật khẩu phải khớp",
      ],
    };
  },
  methods: {
    async submit() {
      const isValid = this.$refs.resetPasswordForm.validate();
      if (isValid) {
        this.loading = true;
        this.errorMessage = "";
        this.successMessage = "";
        try {
          console.log("Reset Code:", this.resetCode); // Debug
          const response = await axios.post(
            "https://localhost:7067/api/Auth/ResetPassword/reset-password",
            {
              code: this.resetCode,
              newPassword: this.newPassword,
              confirmPassword: this.confirmPassword,
            }
          );

          if (response.data.status === 200) {
            this.successMessage =
              response.data.message || "Mật khẩu đã được đặt lại thành công!";
            alert(this.successMessage); // Hiển thị alert thông báo thành công
            this.$router.push({ name: "Login" }); // Chuyển hướng đến trang đăng nhập
          } else {
            this.errorMessage =
              response.data.message || "Đã xảy ra lỗi. Vui lòng thử lại.";
          }
        } catch (error) {
          this.errorMessage =
            error.response?.data?.message ||
            "Đã xảy ra lỗi khi đặt lại mật khẩu.";
        } finally {
          this.loading = false;
        }
      }
    },
    goToLogin() {
      this.$router.push({ name: "Login" });
    },
  },
};
</script>

<style scoped>
.button-login {
  background-color: #87ceeb;
  color: white;
  border-radius: 4px;
}
.button-login:hover {
  background-color: #4682b4;
}
.no-underline {
  text-decoration: none;
}
.auth-wrapper {
  padding: 20px;
}
.auth-card-v2 {
  padding: 2rem;
  border-radius: 8px;
  background-color: #fff;
}
.text-center {
  text-align: center;
}
.text-primary {
  color: blue;
}
.text-base {
  font-size: 1rem;
}
.error-message {
  color: red;
  text-align: center;
}
</style>
