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
              <v-icon large class="mr-2">mdi-home-city</v-icon>
              <span>Chào mừng bạn đến với Website Bất động sản Tuấn Anh</span>
            </v-card-title>
          </v-card>
        </v-col>
      </v-card>
      <v-card class="pa-4 mb-4">
        <v-row>
          <!-- Nhóm 1: Loại sản phẩm, Thời gian bán từ -->
          <v-col cols="4">
            <v-select
              v-model="searchCriteria.type"
              :items="productTypes"
              item-title="text"
              item-value="value"
              label="Loại sản phẩm"
              prepend-icon="mdi-tag-outline"
              outlined
              dense
              class="search-field"
            />
          </v-col>
          <v-col cols="4">
            <v-text-field
              v-model="searchCriteria.startSellingFrom"
              label="Thời gian bán từ"
              type="date"
              prepend-icon="mdi-calendar-clock"
              outlined
              dense
              class="search-field"
            />
          </v-col>
          <v-col cols="4">
            <v-text-field
              v-model="searchCriteria.startSellingTo"
              label="Thời gian bán đến"
              type="date"
              prepend-icon="mdi-calendar-clock"
              outlined
              dense
              class="search-field"
            />
          </v-col>

          <!-- Nhóm 2: Giá tiền từ, Giá tiền đến -->
          <v-col cols="4">
            <v-text-field
              v-model="searchCriteria.priceFrom"
              label="Giá tiền từ"
              prepend-icon="mdi-currency-usd"
              outlined
              dense
              class="search-field"
            />
          </v-col>
          <v-col cols="4">
            <v-text-field
              v-model="searchCriteria.priceTo"
              label="Giá tiền đến"
              prepend-icon="mdi-currency-usd"
              outlined
              dense
              class="search-field"
            />
          </v-col>

          <!-- Nhóm 3: Tìm kiếm theo địa chỉ, Các nút -->
          <v-col cols="4">
            <v-text-field
              v-model="searchCriteria.address"
              label="Tìm kiếm theo địa chỉ"
              prepend-icon="mdi-map-marker"
              outlined
              dense
              class="search-field"
            />
          </v-col>
          <v-col cols="12" class="d-flex align-center justify-end">
            <v-btn @click="resetFilters" class="reset-btn">Reset</v-btn>
            <v-btn @click="fetchProducts" class="search-btn">Tìm kiếm</v-btn>
          </v-col>
        </v-row>
      </v-card>

      <!-- Hiển thị sản phẩm -->
      <v-row>
        <v-col
          v-for="product in getPaginatedProducts()"
          :key="product.id"
          cols="12"
          md="4"
          class="mb-4"
        >
          <v-card class="product-card">
            <v-card-title class="card-title">
              <v-icon>mdi-tag</v-icon> {{ product.title }}
            </v-card-title>
            <v-card-subtitle class="card-subtitle">
              <v-icon>mdi-home-city</v-icon>
              Loại:
              {{
                productTypes.find((type) => type.value === product.typeId)
                  ?.text || "Không xác định"
              }}
            </v-card-subtitle>
            <v-card-text>
              <!-- Hiển thị carousel cho ảnh sản phẩm -->
              <v-carousel hide-delimiters show-arrows height="250" class="mb-4">
                <v-carousel-item
                  v-for="(image, index) in product.productImgs"
                  :key="index"
                >
                  <v-img
                    :src="image.linkImg"
                    @click="showImage(image.linkImg)"
                    height="100%"
                    contain
                  ></v-img>
                </v-carousel-item>
              </v-carousel>
              <div class="my-4 text-subtitle-1">
                <v-icon>mdi-currency-usd</v-icon> Giá:
                {{ formatPrice(product.price) }}
              </div>
              <div><v-icon>mdi-barcode</v-icon> Mã: {{ product.id }}</div>
              <div>
                <v-icon>mdi-calendar-clock</v-icon> Thời gian:
                {{ formatDate(product.startSelling) }}
              </div>
              <div>
                <v-icon>mdi-map-marker</v-icon> Địa chỉ: {{ product.address }}
              </div>
              <v-btn
                @click="showDescription(product.productImgs[0]?.description)"
                class="btn-info"
              >
                <v-icon>mdi-information-outline</v-icon> Xem mô tả
              </v-btn>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Phân trang -->
      <v-row class="mt-4">
        <v-col cols="12">
          <v-pagination
            v-model="currentPage"
            :length="totalPages"
            @input="changePage"
            circle
          ></v-pagination>
        </v-col>
      </v-row>

      <!-- Modal hiển thị mô tả -->
      <v-dialog v-model="descriptionDialog" max-width="600px">
        <v-card>
          <v-card-title>
            <span class="headline">Mô tả</span>
          </v-card-title>
          <v-card-text>
            {{ currentDescription }}
          </v-card-text>
          <v-card-actions>
            <v-btn text @click="descriptionDialog = false">Đóng</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>

      <!-- Modal hiển thị ảnh to -->
      <v-dialog v-model="imageDialog" max-width="80%">
        <v-img :src="currentImage" contain></v-img>
        <v-card-actions>
          <v-btn text @click="imageDialog = false">Đóng</v-btn>
        </v-card-actions>
      </v-dialog>
    </v-col>
  </v-row>
  <CompFooter />
</template>

<script>
import HomePage from "./HomePage.vue";
import CompFooter from "./CompFooter.vue";
import axios from "axios";

export default {
  components: {
    HomePage,
    CompFooter,
  },
  name: "Home",
  data() {
    return {
      loading: false,
      searchCriteria: {
        startSellingFrom: "",
        startSellingTo: "",
        priceFrom: "",
        priceTo: "",
        address: "",
        type: null,
      },
      products: [],
      productTypes: [
        { text: "BDS nhà ở", value: 1 },
        { text: "BDS thương mại", value: 2 },
        { text: "BDS đất đai", value: 3 },
      ],
      descriptionDialog: false,
      imageDialog: false,
      currentDescription: "",
      currentImage: "",
      currentPage: 1,
      itemsPerPage: 12,
      totalPages: 1,
    };
  },
  methods: {
    async fetchProducts() {
      this.loading = true;
      try {
        const token = localStorage.getItem("accessToken");
        const response = await axios.get(
          "https://localhost:7067/api/Product/GetAllProducts",
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        this.products = response.data.data || [];
        this.filterProducts();
      } catch (error) {
        console.error("Error fetching products:", error);
      } finally {
        this.loading = false;
      }
    },
    filterProducts() {
      let filteredProducts = this.products;

      // Lọc theo loại
      if (this.searchCriteria.type) {
        filteredProducts = filteredProducts.filter(
          (product) => product.typeId === this.searchCriteria.type
        );
      }

      // Lọc theo thời gian
      if (this.searchCriteria.startSellingFrom) {
        filteredProducts = filteredProducts.filter(
          (product) =>
            new Date(product.startSelling) >=
            new Date(this.searchCriteria.startSellingFrom)
        );
      }
      if (this.searchCriteria.startSellingTo) {
        filteredProducts = filteredProducts.filter(
          (product) =>
            new Date(product.startSelling) <=
            new Date(this.searchCriteria.startSellingTo)
        );
      }

      // Lọc theo giá
      if (this.searchCriteria.priceFrom) {
        filteredProducts = filteredProducts.filter(
          (product) =>
            product.price >=
            this.convertPriceToNumber(this.searchCriteria.priceFrom)
        );
      }
      if (this.searchCriteria.priceTo) {
        filteredProducts = filteredProducts.filter(
          (product) =>
            product.price <=
            this.convertPriceToNumber(this.searchCriteria.priceTo)
        );
      }

      // Lọc theo địa chỉ
      if (this.searchCriteria.address) {
        filteredProducts = filteredProducts.filter((product) =>
          product.address
            .toLowerCase()
            .includes(this.searchCriteria.address.toLowerCase())
        );
      }

      this.products = filteredProducts;
      this.totalPages = Math.ceil(filteredProducts.length / this.itemsPerPage);
      this.currentPage = 1; // Đặt lại trang hiện tại sau khi lọc
    },
    formatPrice(price) {
      return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
      }).format(price);
    },
    formatDate(date) {
      const options = {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
      };
      return new Date(date).toLocaleDateString("vi-VN", options);
    },
    convertPriceToNumber(price) {
      return parseFloat(price.replace(/[^0-9.-]/g, ""));
    },
    getPaginatedProducts() {
      const start = (this.currentPage - 1) * this.itemsPerPage;
      const end = start + this.itemsPerPage;
      return this.products.slice(start, end);
    },
    resetFilters() {
      this.searchCriteria = {
        startSellingFrom: "",
        startSellingTo: "",
        priceFrom: "",
        priceTo: "",
        address: "",
        type: null,
      };
      this.fetchProducts();
    },
    changePage(page) {
      this.currentPage = page;
    },
    showDescription(description) {
      this.currentDescription = description || "Chưa có mô tả";
      this.descriptionDialog = true;
    },
    showImage(image) {
      this.currentImage = image;
      this.imageDialog = true;
    },
  },
  mounted() {
    this.fetchProducts();
  },
};
</script>

<style scoped>
.title-card {
  padding: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.title-card .v-card-title {
  font-size: 18px;
  font-weight: bold;
}
.title-card .v-icon {
  margin-right: 8px;
}
.content {
  padding-right: 20px;
}
.search-card {
  margin-right: 20px;
}
.product-card {
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s;
}
.product-card:hover {
  transform: translateY(-10px);
}
.card-title {
  font-weight: bold;
  font-size: larger;
}
.card-subtitle {
  color: #666;
}
.search-field {
  margin-bottom: 16px;
}
.reset-btn {
  margin-right: 16px;
  background-color: #ff5722;
  color: #fff;
  border: none;
  border-radius: 4px;
  padding: 8px 16px;
  font-size: 14px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}
.reset-btn:hover {
  background-color: #e64a19;
}
.search-btn {
  background-color: #1976d2;
  color: #fff;
}
.search-btn:hover {
  background-color: #1565c0;
}
.btn-info {
  background-color: #4caf50;
  color: #fff;
}
.btn-info:hover {
  background-color: #388e3c;
}
.product-col {
  margin-right: 20px;
  margin-left: 20px;
}
.v-text-field .v-input__control {
  padding-right: 20px;
}
.v-text-field .v-input__icon--prepend {
  right: 12px;
}
.v-icon {
  margin-right: 8px;
}
</style>
