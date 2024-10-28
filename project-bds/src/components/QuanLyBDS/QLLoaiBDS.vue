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
              <v-icon left class="mr-2">mdi-domain</v-icon>
              <span>Quản Lý Loại Bất Động Sản</span>
            </v-card-title>
          </v-card>
        </v-col>
      </v-card>
      <v-card class="pa-4 mb-4">
        <v-form ref="productTypeForm" v-model="valid" lazy-validation>
          <v-text-field
            v-model="newProductType.code"
            label="Mã Loại"
            :rules="[rules.required]"
          ></v-text-field>
          <v-text-field
            v-model="newProductType.name"
            label="Tên Loại"
            :rules="[rules.required]"
          ></v-text-field>
          <v-btn @click="addProductType" :loading="loading" color="primary"
            >Thêm Loại</v-btn
          >
          <v-btn @click="updateProductType" :loading="loading" color="success"
            >Cập Nhật Loại</v-btn
          >
        </v-form>
      </v-card>

      <v-card class="pa-4 mb-4">
        <v-table class="full-width-table">
          <thead>
            <tr>
              <th class="text-left">Id</th>
              <th class="text-left">Mã Loại</th>
              <th class="text-left">Tên Loại</th>
              <th class="text-left">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="productType in productTypes" :key="productType.id">
              <td>{{ productType.id }}</td>
              <td>{{ productType.code }}</td>
              <td>{{ productType.name }}</td>
              <td>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn
                      @click="editProductType(productType)"
                      icon
                      v-bind="attrs"
                      v-on="on"
                    >
                      <v-icon>mdi-pencil</v-icon>
                    </v-btn>
                  </template>
                  <span>Sửa</span>
                </v-tooltip>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on, attrs }">
                    <v-btn
                      @click="confirmDelete(productType.id)"
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

    <!-- Dialog xác nhận xóa -->
    <v-dialog v-model="confirmDeleteDialog" max-width="500px">
      <v-card>
        <v-card-title>
          <span class="headline">Xác Nhận Xóa</span>
        </v-card-title>
        <v-card-subtitle>
          Bạn có chắc chắn muốn xóa loại bất động sản này không?
        </v-card-subtitle>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="confirmDeleteDialog = false"
            >Hủy</v-btn
          >
          <v-btn
            color="blue darken-1"
            text
            @click="deleteProductType(confirmedProductTypeId)"
            >Đồng Ý</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import HomePage from "../HomePage.vue";
import axios from "axios";

export default {
  components: { HomePage },
  data() {
    return {
      loading: false,
      valid: false,
      productTypes: [],
      newProductType: {
        id: null,
        code: "",
        name: "",
      },
      confirmDeleteDialog: false, // Điều khiển hiển thị dialog xác nhận xóa
      confirmedProductTypeId: null, // ID của loại bất động sản cần xóa
      rules: {
        required: (value) => !!value || "Trường này là bắt buộc",
      },
    };
  },
  methods: {
    async fetchProductTypes() {
      this.loading = true;
      try {
        const token = localStorage.getItem("accessToken");
        if (!token) throw new Error("Token không có hoặc không hợp lệ.");

        const response = await axios.get(
          "https://localhost:7067/api/ProductType/GetAllProductTypes",
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        console.log("API Response:", response.data);

        if (response.status === 200) {
          this.productTypes = response.data.data || response.data;
          console.log("Product Types:", this.productTypes);
        }
      } catch (error) {
        console.error("Error fetching product types:", error);
        if (error.response && error.response.status === 401) {
          alert("Lỗi xác thực: Vui lòng đăng nhập lại.");
          localStorage.removeItem("accessToken");
          localStorage.removeItem("refreshToken");
          this.$router.push("/login");
        } else {
          alert("Lỗi khi lấy danh sách loại bất động sản: " + error.message);
        }
      } finally {
        this.loading = false;
      }
    },
    async addProductType() {
      this.loading = true;
      try {
        const token = localStorage.getItem("accessToken");
        if (!token) throw new Error("Token không có hoặc không hợp lệ.");

        const response = await axios.post(
          "https://localhost:7067/api/ProductType/AddProductType",
          this.newProductType,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        console.log("API Response:", response);

        if (response.status === 200 || response.status === 201) {
          alert("Loại bất động sản đã được thêm thành công.");
          this.fetchProductTypes();
          this.resetForm();
        } else {
          console.error("Lỗi khi thêm loại bất động sản:", response.statusText);
          alert("Lỗi khi thêm loại bất động sản: " + response.statusText);
        }
      } catch (error) {
        if (error.response) {
          console.error("Chi tiết lỗi:", error.response.data);
          alert(
            "Lỗi khi thêm loại bất động sản: " + error.response.data.message ||
              error.response.statusText
          );
        } else {
          console.error("Chi tiết lỗi:", error.message);
          alert("Lỗi khi thêm loại bất động sản: " + error.message);
        }
      } finally {
        this.loading = false;
      }
    },
    async updateProductType() {
      this.loading = true;
      try {
        if (!this.newProductType.id)
          throw new Error("Không có ID loại bất động sản để cập nhật.");

        const token = localStorage.getItem("accessToken");
        if (!token) throw new Error("Token không có hoặc không hợp lệ.");

        const response = await axios.put(
          `https://localhost:7067/api/ProductType/UpdateProductType/${this.newProductType.id}`,
          this.newProductType,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        if (response.status === 200) {
          alert("Loại bất động sản đã được cập nhật thành công.");
          this.fetchProductTypes();
          this.resetForm();
        } else {
          console.error(
            "Lỗi khi cập nhật loại bất động sản:",
            response.statusText
          );
          alert("Lỗi khi cập nhật loại bất động sản.");
        }
      } catch (error) {
        if (error.response) {
          console.error("Chi tiết lỗi:", error.response.data);
          alert(
            "Lỗi khi cập nhật loại bất động sản: " + error.response.data.message
          );
        } else {
          console.error("Chi tiết lỗi:", error.message);
          alert("Lỗi khi cập nhật loại bất động sản: " + error.message);
        }
      } finally {
        this.loading = false;
      }
    },
    async deleteProductType(id) {
      this.loading = true;
      try {
        const token = localStorage.getItem("accessToken");
        if (!token) throw new Error("Token không có hoặc không hợp lệ.");

        const response = await axios.delete(
          `https://localhost:7067/api/ProductType/DeleteProductType/${id}`,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        if (response.status === 200) {
          alert("Loại bất động sản đã được xóa thành công.");
          this.fetchProductTypes();
        } else {
          console.error("Lỗi khi xóa loại bất động sản:", response.statusText);
          alert("Lỗi khi xóa loại bất động sản.");
        }
      } catch (error) {
        if (error.response) {
          console.error("Chi tiết lỗi:", error.response.data);
          alert(
            "Lỗi khi xóa loại bất động sản: " + error.response.data.message
          );
        } else {
          console.error("Chi tiết lỗi:", error.message);
          alert("Lỗi khi xóa loại bất động sản: " + error.message);
        }
      } finally {
        this.loading = false;
        this.confirmDeleteDialog = false; // Đóng hộp thoại sau khi xóa
      }
    },
    resetForm() {
      this.newProductType = {
        id: null,
        code: "",
        name: "",
      };
      this.$refs.productTypeForm.resetValidation();
    },
    editProductType(productType) {
      this.newProductType = { ...productType };
    },
    confirmDelete(id) {
      this.confirmDeleteDialog = true;
      this.confirmedProductTypeId = id;
    },
  },
  created() {
    this.fetchProductTypes();
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
