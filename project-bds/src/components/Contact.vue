<template>
  <v-row>
    <v-col cols="2">
      <HomePage />
    </v-col>
    <v-col cols="10" class="content">
      <v-card class="pa-4 mb-4">
        <v-col cols="12" class="text-center mb-4">
          <v-card class="title-card mx-auto" max-width="600">
            <v-card-title>
              <v-icon large class="mr-2">mdi-message-outline</v-icon>
              <span>Gửi liên hệ đến công ty</span>
            </v-card-title>
          </v-card>
        </v-col>
      </v-card>
      <v-card class="pa-4 mb-4">
        <v-text-field
          v-model="productId"
          label="Nhập mã sản phẩm"
          type="number"
          class="mb-4"
        ></v-text-field>
        <v-btn @click="sendContact" color="primary">Gửi liên hệ</v-btn>
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
import HomePage from "./HomePage.vue";
import axios from "axios";

export default {
  components: {
    HomePage,
  },
  data() {
    return {
      loading: false,
      productId: null, // Mã sản phẩm nhập từ ô nhập
    };
  },
  methods: {
    async sendContact() {
      if (!this.productId) {
        alert('Vui lòng nhập mã sản phẩm.');
        return;
      }

      this.loading = true;

      try {
        // Gửi yêu cầu liên hệ đến API
        const response = await axios.post(
          'https://localhost:7067/api/Contact/SendContact/send',
          { productId: this.productId },
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
              "Content-Type": "application/json", // Đảm bảo rằng loại nội dung là application/json
            },
          }
        );

        if (response.status === 200) {
          alert('Yêu cầu liên hệ đã được gửi thành công!');
          this.productId = null; // Reset ô nhập
        } else {
          alert('Đã xảy ra lỗi khi gửi yêu cầu.');
        }
      } catch (error) {
        if (error.response && error.response.status === 404) {
          alert('Sản phẩm không tồn tại!');
        } else {
          alert('Đã xảy ra lỗi khi gửi yêu cầu.');
        }
      } finally {
        this.loading = false;
      }
    },
  },
};
</script>

<style scoped>
.v-btn {
  margin-top: 16px;
}
</style>
