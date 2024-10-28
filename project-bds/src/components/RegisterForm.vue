<template>
  <v-container>
    <v-form ref="refVForm" v-model="valid" lazy-validation>
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
          <v-card
            v-if="!isConfirm"
            flat
            class="mt-12 mt-sm-0 pa-4"
            max-width="500"
          >
            <v-card-text class="text-center">
              <v-img
                src="../public/icon.jpg"
                :width="100"
                class="mb-6"
                alt="Logo"
              ></v-img>
              <h5 class="text-h5 mb-1">Hành trình bắt đầu tại đây 🚀</h5>
              <p class="mb-0">
                Đăng ký tài khoản để trải nghiệm dịch vụ phía chúng tôi!
              </p>
            </v-card-text>

            <v-card-text>
              <v-form ref="registerForm" @submit.prevent="submit">
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

                  <!-- email -->
                  <v-col cols="12">
                    <v-text-field
                      v-model="email"
                      :rules="emailRules"
                      label="E-mail"
                      required
                    ></v-text-field>
                  </v-col>

                  <!-- full name -->
                  <v-col cols="12">
                    <v-text-field
                      v-model="fullName"
                      :rules="nameRules"
                      label="Họ tên"
                      required
                    ></v-text-field>
                  </v-col>

                  <!-- birth date -->
                  <v-col cols="12">
                    <v-text-field
                      v-model="dateOfBirth"
                      label="Ngày sinh"
                      type="date"
                      :rules="dateOfBirthRules"
                      required
                    ></v-text-field>
                  </v-col>

                  <!-- phone number -->
                  <v-col cols="12">
                    <v-text-field
                      v-model="phoneNumber"
                      label="Số điện thoại"
                      :rules="phoneNumberRules"
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

                  <!-- confirm password -->
                  <v-col cols="12">
                    <v-text-field
                      v-model="confirmPassword"
                      :append-icon="
                        showConfirmPassword ? 'mdi-eye' : 'mdi-eye-off'
                      "
                      :type="showConfirmPassword ? 'text' : 'password'"
                      @click:append="showConfirmPassword = !showConfirmPassword"
                      :rules="confirmPasswordRules"
                      label="Xác nhận mật khẩu"
                      required
                    ></v-text-field>
                  </v-col>

                  <!-- gender -->
                  <v-col cols="12">
                    <v-label>Giới tính</v-label>
                    <v-radio-group v-model="gender" :rules="genderRules" row>
                      <v-radio label="Nam" value="male"></v-radio>
                      <v-radio label="Nữ" value="female"></v-radio>
                    </v-radio-group>
                  </v-col>

                  <!-- action -->
                  <v-col cols="12">
                    <v-btn
                      :disabled="loading"
                      block
                      :loading="loading"
                      @click="submit"
                      class="button-style"
                    >
                      Đăng ký
                    </v-btn>
                  </v-col>
                  <v-alert v-if="errorMessage" type="error">{{
                    errorMessage
                  }}</v-alert>

                  <!-- login link -->
                  <v-col cols="12" class="text-center text-base">
                    <span>Đã có tài khoản?</span>
                    <router-link
                      class="text-primary ms-2 no-underline"
                      to="/login"
                    >
                      <v-btn class="button-secondary" text>Đăng nhập</v-btn>
                    </router-link>
                  </v-col>
                </v-row>
              </v-form>
            </v-card-text>
          </v-card>
          <v-card v-else flat class="mt-12 mt-sm-0 pa-4" max-width="500">
            <v-card-text class="text-center">
              <v-img
                src="../public/icon.jpg"
                :width="100"
                class="mb-6"
                alt="Logo"
              ></v-img>
              <h5 class="text-h5 mb-1">
                Mã xác nhận đã được gửi đến email của bạn 🚀
              </h5>
              <p class="mb-0">
                Vui lòng kiểm tra email và nhập mã xác nhận của bạn
              </p>
            </v-card-text>
            <v-card-text>
              <v-form ref="confirmForm" @submit.prevent="onConfirm">
                <v-row>
                  <v-col cols="12">
                    <v-text-field
                      v-model="confirmCode"
                      label="Mã xác nhận"
                      required
                    ></v-text-field>
                  </v-col>
                  <v-col cols="12">
                    <v-btn
                      :disabled="loading"
                      block
                      :loading="loading"
                      @click="onConfirm"
                      class="button-style"
                    >
                      Xác nhận
                    </v-btn>
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
    email: "",
    name: "",
    dateOfBirth: "",
    phoneNumber: "",
    password: "",
    confirmPassword: "",
    fullName: "",
    gender: "",
    showPassword: false,
    showConfirmPassword: false,
    loading: false,
    valid: false,
    isConfirm: false,
    confirmCode: "",
    usernameRules: [(v) => !!v || "Tài khoản là bắt buộc"],
    emailRules: [
      (v) => !!v || "E-mail là bắt buộc",
      (v) => /.+@.+\..+/.test(v) || "E-mail không hợp lệ",
    ],
    nameRules: [(v) => !!v || "Họ tên là bắt buộc"],
    dateOfBirthRules: [(v) => !!v || "Ngày sinh là bắt buộc"],
    phoneNumberRules: [(v) => !!v || "Số điện thoại là bắt buộc"],
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
      (v) => v === this.password || "Mật khẩu không khớp!",
    ],
    genderRules: [(v) => !!v || "Giới tính là bắt buộc"],
  }),
  methods: {
    async submit() {
      const isValid = this.$refs.registerForm.validate();
      if (isValid) {
        this.loading = true;
        const payload = {
          username: this.username,
          password: this.password,
          confirmPassword: this.confirmPassword,
          email: this.email,
          phoneNumber: this.phoneNumber,
          dateOfBirth: this.dateOfBirth,
          fullName: this.fullName,
          gender: this.gender === "male" ? true : false,
        };
        console.log("Sending registration payload:", payload);
        try {
          const response = await axios.post(
            "https://localhost:7067/api/Auth/Register",
            payload
          );
          if (response.data.status === 201) {
            alert(response.data.message);
            this.isConfirm = true; 
          } else {
            alert(response.data.message || "Đã xảy ra lỗi. Vui lòng thử lại.");
          }
        } catch (error) {
          alert("Lỗi khi kết nối đến máy chủ. Vui lòng thử lại sau.");
        } finally {
          this.loading = false;
        }
      }
    },
    async onConfirm() {
      if (this.confirmCode) {
        this.loading = true;
        const payload = {
          confirmCode: this.confirmCode,
        };
        try {
          const response = await axios.post(
            "https://localhost:7067/api/Auth/ConfirmEmail",
            payload
          );
          alert(response.data.message);
          this.$router.push("/login"); 
        } catch (error) {
          if (
            error.response &&
            error.response.data &&
            error.response.data.message
          ) {
            alert(error.response.data.message);
          } else {
            alert("Đã xảy ra lỗi khi xác nhận tài khoản.");
          }
        } finally {
          this.loading = false;
        }
      } else {
        alert("Vui lòng nhập mã xác nhận");
      }
    },
  },
};
</script>

<style scoped>
.button-style {
  background-color: #87ceeb;
  color: white;
  border-radius: 4px;
}

.button-style:hover {
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
</style>
