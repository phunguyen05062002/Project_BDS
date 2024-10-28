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
              <v-icon left class="mr-2">mdi-image-album</v-icon>
              <span>Quản Lý Hình Ảnh Bất Động Sản</span>
            </v-card-title>
          </v-card>
        </v-col>
      </v-card>
      <v-card class="pa-4 mb-4">
        <v-row align="center" class="mb-4">
          <v-col cols="4" class="d-flex">
            <v-btn @click="openDialog('add')" color="primary" class="mr-2">
              Thêm Ảnh
            </v-btn>
          </v-col>
          <v-col cols="8" class="d-flex justify-end">
            <v-text-field
              v-model="searchProductId"
              label="Tìm kiếm theo ProductId"
              @keyup.enter="searchImages"
              dense
              hide-details
            ></v-text-field>
          </v-col>
        </v-row>
      </v-card>
      <v-card class="pa-4 mb-4">
        <v-table class="full-width-table">
          <thead>
            <tr>
              <th class="text-left">Id</th>
              <th class="text-left">ProductId</th>
              <th class="text-left">LinkImg</th>
              <th class="text-left">Description</th>
              <th class="text-left">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="filteredImages.length === 0">
              <td colspan="5" class="text-center">Không có dữ liệu</td>
            </tr>
            <tr v-for="image in paginatedImages" :key="image.id">
              <td>{{ image.id }}</td>
              <td>{{ image.productId }}</td>
              <td>
                <a :href="image.linkImg" target="_blank">{{ image.linkImg }}</a>
              </td>
              <td>{{ image.description }}</td>
              <td>
                <v-btn @click="openDialog('edit', image)" icon>
                  <v-icon>mdi-pencil</v-icon>
                </v-btn>
                <v-btn @click="confirmDelete(image)" icon>
                  <v-icon>mdi-delete</v-icon>
                </v-btn>
              </td>
            </tr>
          </tbody>
        </v-table>
        <v-pagination
          v-if="totalPages > 1"
          v-model="currentPage"
          :length="totalPages"
          next-icon="mdi-menu-right"
          prev-icon="mdi-menu-left"
          :total-visible="5"
          @input="changePage"
        ></v-pagination>
        <span v-if="totalPages > 1"
          >Trang {{ currentPage }} / {{ totalPages }}</span
        >
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

      <!-- Dialog Thêm/Sửa -->
      <v-dialog v-model="dialog.visible" max-width="500px">
        <v-card>
          <v-card-title>
            <span class="headline">{{
              dialog.type === "add" ? "Thêm Ảnh" : "Sửa Ảnh"
            }}</span>
          </v-card-title>
          <v-card-subtitle>
            <v-form ref="form" v-model="formValid">
              <v-text-field
                v-model="dialog.data.linkImg"
                label="Link Ảnh"
                required
              ></v-text-field>
              <v-text-field
                v-model="dialog.data.description"
                label="Mô Tả"
                required
              ></v-text-field>
              <!-- Trường ProductId chỉ hiển thị, không cho chỉnh sửa -->
              <v-text-field
                v-model="dialog.data.productId"
                label="ProductId"
                type="number"
                :disabled="dialog.type === 'edit'"
                required
              ></v-text-field>
            </v-form>
          </v-card-subtitle>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn text @click="closeDialog">Hủy</v-btn>
            <v-btn color="primary" @click="submitForm">
              {{ dialog.type === "add" ? "Thêm" : "Cập Nhật" }}
            </v-btn>
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
            Bạn có chắc chắn muốn xóa ảnh này không?
          </v-card-subtitle>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn text @click="closeConfirmDelete">Hủy</v-btn>
            <v-btn color="red" @click="deleteImage">Xóa</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-col>
  </v-row>
</template>

<script>
import axios from "axios";
import HomePage from "../HomePage.vue";

export default {
  components: {
    HomePage,
  },
  name: "ImageManagement",
  data() {
    return {
      loading: false,
      images: [],
      filteredImages: [], // Dữ liệu đã được lọc theo tìm kiếm
      currentPage: 1,
      perPage: 10,
      searchProductId: "", // Giá trị tìm kiếm theo productId
      dialog: {
        visible: false,
        type: "",
        data: {
          id: null,
          linkImg: "",
          description: "",
          productId: null,
        },
      },
      confirmDeleteDialog: false,
      imageToDelete: null,
      formValid: false,
    };
  },
  computed: {
    paginatedImages() {
      const start = (this.currentPage - 1) * this.perPage;
      const end = start + this.perPage;
      return this.filteredImages.slice(start, end);
    },
    totalPages() {
      return Math.ceil(this.filteredImages.length / this.perPage);
    },
  },
  methods: {
    setupAxiosInterceptors() {
      axios.interceptors.request.use(
        (config) => {
          const token = localStorage.getItem("accessToken");
          if (token) {
            config.headers.Authorization = `Bearer ${token}`;
          }
          return config;
        },
        (error) => {
          return Promise.reject(error);
        }
      );
    },
    async fetchImages() {
      this.loading = true;
      try {
        const response = await axios.get(
          "https://localhost:7067/api/ProductImg/GetAllProductImages"
        );
        this.images = response.data.data;
        this.filteredImages = [...this.images]; // Gán dữ liệu tìm kiếm bằng dữ liệu ban đầu
        this.changePage(1); // Reset to page 1 after fetch
      } catch (error) {
        console.error("Error fetching images:", error);
      } finally {
        this.loading = false;
      }
    },
    openDialog(type, image = null) {
      this.dialog.type = type;
      if (type === "edit" && image) {
        this.dialog.data = { ...image };
        this.dialog.data.productId = image.productId;
      } else {
        this.dialog.data = {
          id: null,
          linkImg: "",
          description: "",
          productId: null,
        };
      }
      this.dialog.visible = true;
    },
    closeDialog() {
      this.dialog.visible = false;
    },
    async submitForm() {
      if (this.$refs.form.validate()) {
        this.loading = true;
        try {
          if (this.dialog.type === "add") {
            await axios.post(
              "https://localhost:7067/api/ProductImg/AddProductImage",
              this.dialog.data
            );
          } else if (this.dialog.type === "edit") {
            await axios.put(
              `https://localhost:7067/api/ProductImg/UpdateProductImage/${this.dialog.data.id}`,
              {
                ...this.dialog.data,
                productId: undefined,
              }
            );
          }
          this.fetchImages();
          this.closeDialog();
        } catch (error) {
          console.error("Error submitting form:", error);
        } finally {
          this.loading = false;
        }
      }
    },
    confirmDelete(image) {
      this.imageToDelete = image;
      this.confirmDeleteDialog = true;
    },
    closeConfirmDelete() {
      this.confirmDeleteDialog = false;
      this.imageToDelete = null;
    },
    async deleteImage() {
      if (this.imageToDelete) {
        this.loading = true;
        try {
          await axios.delete(
            `https://localhost:7067/api/ProductImg/DeleteProductImage/${this.imageToDelete.id}`
          );
          this.fetchImages();
        } catch (error) {
          console.error("Error deleting image:", error);
        } finally {
          this.loading = false;
          this.closeConfirmDelete();
        }
      }
    },
    searchImages() {
      if (this.searchProductId.trim() === "") {
        this.filteredImages = [...this.images];
        this.changePage(1); // Reset to page 1 when search input is cleared
      } else {
        // Thực hiện tìm kiếm trong dữ liệu đã tải về
        this.filteredImages = this.images.filter((image) =>
          image.productId.toString().includes(this.searchProductId)
        );
        if (this.filteredImages.length === 0) {
          this.currentPage = 1; // Reset to page 1 if no results
        }
        this.changePage(1); // Reset to page 1 after search
      }
    },
    changePage(page) {
      this.currentPage = page;
    },
  },
  mounted() {
    this.setupAxiosInterceptors();
    this.fetchImages();
  },
};
</script>

<style scoped>
.full-width-table {
  width: 100%;
  table-layout: fixed;
}
.text-center {
  text-align: center;
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
