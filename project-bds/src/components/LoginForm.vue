<template>
  <v-container>
    <v-form ref="loginForm" @submit.prevent="submit">
      <v-row no-gutters class="auth-wrapper bg-surface">
        <v-col
          lg="8"
          style="background-image: url(../public/Banner-1.png)"
        ></v-col>
        <v-col
          cols="12"
          lg="4"
          class="auth-card-v2 d-flex align-center justify-center"
        >
          <v-card flat class="mt-12 mt-sm-0 pa-4" max-width="500">
            <v-card-text class="text-center">
              <v-img
                src="../public/icon.jpg"
                :width="100"
                class="mb-6"
                alt="Logo"
              ></v-img>
              <h5 class="text-h5 mb-1">Chào mừng bạn đến BĐS Tuấn Anh!</h5>
              <p class="mb-0">
                Đăng nhập vào tài khoản của bạn để trải nghiệm.
              </p>
            </v-card-text>

            <v-card-text>
              <v-form ref="loginForm" @submit.prevent="submit">
                <v-row>
                  <!-- username -->
                  <v-col cols="12">
                    <v-text-field
                      v-model="username"
                      :rules="usernameRules"
                      label="Tài khoản"
                      required
                    ></v-text-field>
                  </v-col>

                  <!-- password -->
                  <v-col cols="12">
                    <v-text-field
                      v-model="password"
                      :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                      :type="showPassword ? 'text' : 'password'"
                      @click:append="showPassword = !showPassword"
                      :rules="passwordRules"
                      label="Mật khẩu"
                      required
                    ></v-text-field>
                  </v-col>

                  <!-- remember me and forgot password -->
                  <v-col cols="12">
                    <div class="d-flex justify-space-between align-center">
                      <v-checkbox
                        v-model="rememberMe"
                        label="Lưu mật khẩu"
                      ></v-checkbox>
                      <router-link
                        class="text-primary no-underline"
                        to="/forgot-password"
                        >Quên mật khẩu?</router-link
                      >
                    </div>
                  </v-col>

                  <!-- action -->
                  <v-col cols="12">
                    <v-btn
                      block
                      :loading="loading"
                      class="button-login"
                      type="submit"
                      >Đăng nhập</v-btn
                    >
                  </v-col>

                  <!-- error message -->
                  <v-col cols="12" v-if="errorMessage">
                    <v-alert type="error">{{ errorMessage }}</v-alert>
                  </v-col>

                  <!-- create account -->
                  <v-col cols="12" class="text-center text-base">
                    <span>Bạn chưa có tài khoản?</span>
                    <router-link
                      class="text-primary ms-2 no-underline"
                      to="/register"
                    >
                      <v-btn class="button-secondary" text>Đăng ký</v-btn>
                    </router-link>
                  </v-col>
                </v-row>
              </v-form>
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
    username: "",
    password: "",
    rememberMe: false,
    showPassword: false,
    loading: false,
    errorMessage: "",
    usernameRules: [(v) => !!v || "Tài khoản là bắt buộc"],
    passwordRules: [
      (v) => !!v || "Mật khẩu là bắt buộc",
      (v) =>
        /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/.test(
          v
        ) ||
        "Mật khẩu phải có ít nhất 8 ký tự, ít nhất 1 ký tự in hoa, ít nhất 1 ký tự in thường, ít nhất 1 số và 1 ký tự đặc biệt!",
    ],
  }),
  methods: {
    async submit() {
      const isValid = this.$refs.loginForm.validate();
      if (isValid) {
        this.loading = true;
        this.errorMessage = "";
        try {
          const response = await axios.post(
            "https://localhost:7067/api/Auth/Login",
            {
              username: this.username,
              password: this.password,
            }
          );

          if (response.status === 200 && response.data && response.data.data) {
            const storage = this.rememberMe ? localStorage : sessionStorage;

            localStorage.setItem("accessToken", response.data.data.accessToken);
            localStorage.setItem(
              "refreshToken",
              response.data.data.refreshToken
            );
            localStorage.setItem("roleId", response.data.data.roleId);
            localStorage.setItem("userId", response.data.data.userId);

            alert("Đăng nhập thành công!");
            this.$router.push("/trang-chu");
          } else {
            this.errorMessage =
              response.data.message || "Thông tin tài khoản không đúng.";
          }
        } catch (error) {
          this.errorMessage =
            error.response?.data?.message || "Đã xảy ra lỗi khi đăng nhập.";
        } finally {
          this.loading = false;
        }
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
.button-secondary {
  background-color: gray;
  color: white;
  border-radius: 4px;
  font-weight: bold;
}
.button-secondary:hover {
  background-color: darkgray;
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
