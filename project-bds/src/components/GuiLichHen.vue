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
              <v-icon large class="mr-2">mdi-calendar-plus</v-icon>
              <span>Gửi lịch hẹn đến khách hàng</span>
            </v-card-title>
          </v-card>
        </v-col>
      </v-card>

      <v-card class="pa-4 mb-4">
        <v-table class="full-width-table">
          <thead>
            <tr>
              <th class="text-left">Id</th>
              <th class="text-left">ProductId</th>
              <th class="text-left">Create Time</th>
              <th class="text-left">Status</th>
              <th class="text-left">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="contact in contacts" :key="contact.id">
              <td>{{ contact.id }}</td>
              <td>{{ contact.productId }}</td>
              <td>{{ new Date(contact.createTime).toLocaleString() }}</td>
              <td>
                <v-icon :color="contact.isStatus ? 'green' : 'red'">
                  {{
                    contact.isStatus ? "mdi-check-circle" : "mdi-alert-circle"
                  }}
                </v-icon>
              </td>
              <td>
                <v-btn @click="sendNotification(contact.id)" icon>
                  <v-icon>mdi-bell</v-icon>
                </v-btn>
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
  name: "ContactManagement",
  data() {
    return {
      loading: false,
      contacts: [],
    };
  },
  async created() {
    await this.fetchContacts();
  },
  methods: {
    async fetchContacts() {
      this.loading = true;
      try {
        const accessToken = localStorage.getItem("accessToken");
        const response = await axios.get(
          "https://localhost:7067/api/Contact/GetContactsForRoleId3/GetContactsForRoleId3",
          {
            headers: {
              Authorization: `Bearer ${accessToken}`,
            },
          }
        );
        if (response.status === 200) {
          this.contacts = response.data.data;
        } else {
          alert("Lỗi khi tải danh sách liên hệ.");
        }
      } catch (error) {
        console.error("Đã xảy ra lỗi khi tải danh sách liên hệ.", error);
        alert("Đã xảy ra lỗi khi tải danh sách liên hệ.");
      } finally {
        this.loading = false;
      }
    },
    async sendNotification(contactId) {
      this.loading = true;
      try {
        const accessToken = localStorage.getItem("accessToken");
        const response = await axios.post(
          `https://localhost:7067/api/Notification/SendNotification/send/${contactId}`,
          {},
          {
            headers: {
              Authorization: `Bearer ${accessToken}`,
            },
          }
        );
        if (response.status === 200) {
          alert("Thông báo đã được gửi thành công!");
          // Cập nhật trạng thái để phản ánh sự thay đổi
          await this.fetchContacts();
        } else {
          alert("Lỗi khi gửi thông báo.");
        }
      } catch (error) {
        console.error("Đã xảy ra lỗi khi gửi thông báo.", error);
        alert("Đã xảy ra lỗi khi gửi thông báo.");
      } finally {
        this.loading = false;
      }
    },
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
