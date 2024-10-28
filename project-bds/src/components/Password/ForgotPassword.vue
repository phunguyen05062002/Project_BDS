<template>
  <v-container>
    <v-form ref="forgotPasswordForm" @submit.prevent="submit">
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
              <h5 class="text-h5 mb-1">Quên mật khẩu?</h5>
              <p class="mb-0">
                Nhập email của bạn để nhận mã đặt lại mật khẩu.
              </p>
            </v-card-text>

            <v-card-text>
              <v-row>
                <!-- Email -->
                <v-col cols="12">
                  <v-text-field
                    v-model="email"
                    :rules="emailRules"
                    label="Email"
                    required
                  ></v-text-field>
                </v-col>

                <!-- Action -->
                <v-col cols="12">
                  <v-btn
                    block
                    :loading="loading"
                    class="button-login"
                    type="submit"
                    >Gửi mã đặt lại mật khẩu</v-btn
                  >
                </v-col>

                <!-- Error Message -->
                <v-col cols="12" v-if="errorMessage">
                  <v-alert type="error" dismissible v-model="errorMessage">
                    {{ errorMessage }}
                  </v-alert>
                </v-col>

                <!-- Success Message -->
                <v-col cols="12" v-if="successMessage">
                  <v-alert type="success" dismissible v-model="successMessage">
                    {{ successMessage }}
                  </v-alert>
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
  data: () => ({
    email: "",
    loading: false,
    errorMessage: "",
    successMessage: "",
    emailRules: [
      (v) => !!v || "Email là bắt buộc",
      (v) => /.+@.+\..+/.test(v) || "Email không hợp lệ",
    ],
  }),
  methods: {
    async submit() {
      this.loading = true;
      this.errorMessage = "";
      this.successMessage = "";

      try {
        const response = await axios.post(
          "https://localhost:7067/api/Auth/ForgotPassword/forgot-password",
          { email: this.email }
        );

        // Kiểm tra mã trạng thái HTTP từ API
        if (response.status === 200) {
          this.successMessage = response.data.message;
          setTimeout(() => {
            this.$router.push("/reset-password");
          }, 2000);
        }
      } catch (error) {
        // Xử lý lỗi và hiển thị thông báo từ backend
        if (
          error.response &&
          error.response.data &&
          error.response.data.message
        ) {
          this.errorMessage = error.response.data.message;
        } else {
          this.errorMessage = "Đã xảy ra lỗi không xác định. Vui lòng thử lại.";
        }
      } finally {
        this.loading = false;
      }
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
