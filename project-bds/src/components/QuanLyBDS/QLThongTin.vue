<template>
  <v-row>
    <v-col cols="2">
      <HomePage />
    </v-col>
    <v-col cols="10" class="content">
      <!-- Card tiêu đề -->
      <v-card class="pa-4 mb-4">
        <v-row>
          <v-col cols="12" class="text-center">
            <v-card class="title-card mx-auto" max-width="600">
              <v-card-title>
                <v-icon large>mdi-home-city</v-icon>

                <span> Quản Lý Thông Tin Bất Động Sản </span>
              </v-card-title>
            </v-card>
          </v-col>
        </v-row>
      </v-card>

      <!-- Phần tìm kiếm và thêm sản phẩm -->
      <v-card class="pa-4 mb-4">
        <v-form ref="form">
          <v-row>
            <!-- Hàng 1 -->
            <v-col cols="4">
              <v-text-field
                v-model="searchCriteria.startSellingFrom"
                label="Thời gian bán từ"
                type="date"
                outlined
                dense
              />
            </v-col>
            <v-col cols="4">
              <v-text-field
                v-model="searchCriteria.startSellingTo"
                label="Thời gian bán đến"
                type="date"
                outlined
                dense
              />
            </v-col>
            <v-col cols="4">
              <v-select
                v-model="searchCriteria.address"
                :items="provinces"
                label="Địa chỉ"
                outlined
                dense
              />
            </v-col>
          </v-row>
          <v-row>
            <!-- Hàng 2 -->
            <v-col cols="4">
              <v-select
                v-model="searchCriteria.typeId"
                :items="productTypes"
                item-title="name"
                item-value="id"
                label="Loại bất động sản"
                outlined
                dense
              />
            </v-col>
            <v-col cols="4">
              <v-text-field
                v-model="searchCriteria.minPrice"
                label="Giá tối thiểu"
                type="number"
                outlined
                dense
              />
            </v-col>
            <v-col cols="4">
              <v-text-field
                v-model="searchCriteria.maxPrice"
                label="Giá tối đa"
                type="number"
                outlined
                dense
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="6">
              <v-btn @click="applySearch" color="primary" class="mt-4"
                >Tìm kiếm</v-btn
              >
            </v-col>
            <v-col cols="6" class="text-right">
              <v-btn
                @click="openAddEditDialog"
                color="blue darken-1"
                class="mt-4"
                >Thêm Sản Phẩm</v-btn
              >
            </v-col>
          </v-row>
        </v-form>
      </v-card>

      <!-- Danh sách sản phẩm -->
      <v-card class="pa-4 mb-4">
        <v-table class="full-width-table">
          <thead>
            <tr>
              <th class="text-left font-weight-bold">Id</th>
              <th class="text-left font-weight-bold">Tên người bán</th>
              <th class="text-left font-weight-bold">
                Số điện thoại người bán
              </th>
              <th class="text-left font-weight-bold">Địa chỉ</th>
              <th class="text-left font-weight-bold">Tiêu đề</th>
              <th class="text-left font-weight-bold">Giá</th>
              <th class="text-left font-weight-bold">Loại</th>
              <th class="text-left font-weight-bold">Ngày bắt đầu bán</th>
              <th class="text-left font-weight-bold">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="product in paginatedData" :key="product.id">
              <td>{{ product.id }}</td>
              <td>{{ product.hostName }}</td>
              <td>{{ product.hostPhoneNumber }}</td>
              <td>{{ product.address }}</td>
              <td>{{ product.title }}</td>
              <td>{{ product.price }}</td>
              <td>{{ getProductTypeName(product.typeId) }}</td>
              <td>{{ product.startSelling }}</td>
              <td>
                <v-btn @click="openAddEditDialog(product)" icon>
                  <v-icon>mdi-pencil</v-icon>
                </v-btn>
                <v-btn @click="confirmDeleteProduct(product)" icon>
                  <v-icon>mdi-delete</v-icon>
                </v-btn>
              </td>
            </tr>
          </tbody>
        </v-table>
      </v-card>

      <!-- Phân trang -->
        <v-pagination
          v-model="currentPage"
          :length="totalPages"
          next-icon="mdi-menu-right"
          prev-icon="mdi-menu-left"
          @input="fetchProducts"
        ></v-pagination>
        <span>Trang {{ currentPage }} / {{ totalPages }}</span>

      <!-- Loader -->
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

      <!-- Modal Thêm/Sửa Sản Phẩm -->
      <v-dialog v-model="dialogVisible" max-width="600px">
        <v-card>
          <v-card-title>
            <span class="headline">{{
              currentProduct.id ? "Sửa Sản Phẩm" : "Thêm Sản Phẩm"
            }}</span>
          </v-card-title>
          <v-card-text>
            <v-form ref="form">
              <v-text-field
                v-model="currentProduct.hostName"
                label="Tên người bán"
                outlined
                dense
              />
              <v-text-field
                v-model="currentProduct.hostPhoneNumber"
                label="Số điện thoại người bán"
                outlined
                dense
              />
              <v-text-field
                v-model="currentProduct.address"
                label="Địa chỉ"
                outlined
                dense
              />
              <v-text-field
                v-model="currentProduct.description"
                label="Mô tả"
                outlined
                dense
              />
              <v-text-field
                v-model="currentProduct.price"
                label="Giá"
                type="number"
                outlined
                dense
              />
              <v-select
                v-model="currentProduct.typeId"
                :items="productTypes"
                item-title="name"
                item-value="id"
                label="Loại sản phẩm"
                outlined
                dense
              />
              <v-text-field
                v-model="currentProduct.startSelling"
                label="Ngày bắt đầu bán"
                type="date"
                outlined
                dense
              />
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" text @click="dialogVisible = false"
              >Hủy bỏ</v-btn
            >
            <v-btn color="blue darken-1" text @click="saveProduct">Lưu</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>

      <!-- Dialog Xác Nhận Xóa -->
      <v-dialog v-model="confirmDeleteDialog" max-width="500px">
        <v-card>
          <v-card-title>
            <span class="headline">Xác Nhận Xóa</span>
          </v-card-title>
          <v-card-subtitle>
            Bạn có chắc chắn muốn xóa sản phẩm này không?
          </v-card-subtitle>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn text @click="closeConfirmDelete">Hủy</v-btn>
            <v-btn color="red" @click="deleteProduct">Xóa</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-col>
  </v-row>
</template>

<script>
import HomePage from "../HomePage.vue";
import axios from "axios";

export default {
  components: {
    HomePage,
  },
  name: "ProductManagement",
  data() {
    return {
      loading: false,
      products: [], // To store all products
      productTypes: [
        { id: 1, name: "BDS nhà ở" },
        { id: 2, name: "BDS thương mại" },
        { id: 3, name: "BDS đất đai" },
      ],
      provinces: ["Hà Nội", "Nam Định", "Hải Phòng"],
      addresses: ["Hà Nội", "Nam định", "Hải phòng"],
      searchCriteria: {
        address: "",
        minPrice: "",
        maxPrice: "",
        typeId: null,
        startSellingFrom: "",
        startSellingTo: "",
      },
      currentPage: 1,
      perPage: 10,
      dialogVisible: false,
      confirmDeleteDialog: false,
      currentProduct: null,
    };
  },
  computed: {
    paginatedData() {
      const start = (this.currentPage - 1) * this.perPage;
      const end = start + this.perPage;
      return this.filteredProducts.slice(start, end);
    },
    totalPages() {
      return Math.ceil(this.filteredProducts.length / this.perPage);
    },
    filteredProducts() {
      return this.products.filter((product) => {
        // Kiểm tra tỉnh có trong địa chỉ
        const addressContainsProvince = this.searchCriteria.address
          ? this.provinces.some(
              (province) =>
                product.address.includes(province) &&
                this.searchCriteria.address === province
            )
          : true;

        return (
          addressContainsProvince &&
          (!this.searchCriteria.typeId ||
            product.typeId === this.searchCriteria.typeId) &&
          (!this.searchCriteria.minPrice ||
            product.price >= this.searchCriteria.minPrice) &&
          (!this.searchCriteria.maxPrice ||
            product.price <= this.searchCriteria.maxPrice) &&
          (!this.searchCriteria.startSellingFrom ||
            new Date(product.startSelling) >=
              new Date(this.searchCriteria.startSellingFrom)) &&
          (!this.searchCriteria.startSellingTo ||
            new Date(product.startSelling) <=
              new Date(this.searchCriteria.startSellingTo))
        );
      });
    },
  },
  methods: {
    getProductTypeName(typeId) {
      const type = this.productTypes.find((type) => type.id === typeId);
      return type ? type.name : "Chưa xác định";
    },
    async fetchProducts() {
      try {
        this.loading = true;
        const token = localStorage.getItem("accessToken");
        if (!token) {
          alert("Token không có. Vui lòng đăng nhập lại.");
          this.$router.push("/login");
          return;
        }

        const response = await axios.get(
          `https://localhost:7067/api/Product/GetAllProducts?page=${this.currentPage}&size=${this.perPage}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        this.products = response.data.data;
        this.totalPages = Math.ceil(response.data.totalPages / this.perPage);
      } catch (error) {
        console.error("Lỗi khi tải danh sách sản phẩm: ", error);
        if (error.response && error.response.status === 401) {
          alert("Lỗi xác thực: Vui lòng đăng nhập lại.");
          localStorage.removeItem("accessToken");
          this.$router.push("/login");
        }
      } finally {
        this.loading = false;
      }
    },
    async deleteProduct() {
      try {
        const token = localStorage.getItem("accessToken");
        await axios.delete(
          `https://localhost:7067/api/Product/DeleteProduct/${this.currentProduct.id}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        this.fetchProducts(); // Refresh products after deletion
        this.confirmDeleteDialog = false; // Close the confirmation dialog
      } catch (error) {
        console.error("Lỗi khi xóa sản phẩm: ", error);
        if (error.response && error.response.status === 401) {
          alert("Lỗi xác thực: Vui lòng đăng nhập lại.");
          localStorage.removeItem("accessToken");
          this.$router.push("/login");
        } else {
          alert("Lỗi khi xóa sản phẩm: " + error.message);
        }
      }
    },
    confirmDeleteProduct(product) {
      this.currentProduct = product;
      this.confirmDeleteDialog = true; // Show the confirmation dialog
    },
    closeConfirmDelete() {
      this.confirmDeleteDialog = false; // Close the confirmation dialog
      this.currentProduct = null; // Clear currentProduct
    },
    openAddEditDialog(product = null) {
      this.currentProduct = product ? { ...product } : {}; // Set currentProduct for adding or editing
      this.dialogVisible = true; // Show the dialog
    },
    async saveProduct() {
      try {
        const token = localStorage.getItem("accessToken");
        if (!token) {
          alert("Token không có. Vui lòng đăng nhập lại.");
          this.$router.push("/login");
          return;
        }

        if (this.currentProduct.id) {
          // Cập nhật sản phẩm
          await axios.put(
            `https://localhost:7067/api/Product/UpdateProduct/${this.currentProduct.id}`,
            this.currentProduct,
            {
              headers: {
                Authorization: `Bearer ${token}`,
                "Content-Type": "application/json", // Đảm bảo kiểu nội dung là JSON
              },
            }
          );
        } else {
          // Thêm sản phẩm mới
          await axios.post(
            `https://localhost:7067/api/Product/AddProduct`,
            this.currentProduct,
            {
              headers: {
                Authorization: `Bearer ${token}`,
                "Content-Type": "application/json", // Đảm bảo kiểu nội dung là JSON
              },
            }
          );
        }
        this.fetchProducts(); // Làm mới danh sách sản phẩm sau khi lưu
        this.dialogVisible = false; // Đóng hộp thoại
      } catch (error) {
        console.error("Lỗi khi lưu sản phẩm: ", error);
        if (error.response && error.response.status === 401) {
          alert("Lỗi xác thực: Vui lòng đăng nhập lại.");
          localStorage.removeItem("accessToken");
          this.$router.push("/login");
        } else {
          alert("Lỗi khi lưu sản phẩm: " + error.message);
        }
      }
    },
    resetSearch() {
      this.searchCriteria = {
        address: "",
        minPrice: "",
        maxPrice: "",
        typeId: null,
        startSellingFrom: "",
        startSellingTo: "",
      };
      this.currentPage = 1;
      this.fetchProducts();
    },

    applySearch() {
      this.currentPage = 1;
      this.paginatedData;
    },
  },
  created() {
    this.fetchProducts();
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
}

.content {
  padding: 16px;
}
</style>
