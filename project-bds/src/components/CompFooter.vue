<template>
    <v-footer class="d-flex flex-column">
      <div class="bg-green-lighten-1 d-flex w-100 align-center px-4 py-2">
        <strong>Kết nối với chúng tôi qua các mạng xã hội!</strong>
  
        <v-spacer></v-spacer>
  
        <v-btn
          v-for="icon in icons"
          :key="icon"
          :icon="icon"
          class="mx-2"
          size="small"
          variant="plain"
          color="white"
        ></v-btn>
      </div>
  
      <div class="px-4 py-3 bg-yellow-lighten-4 text-center w-100">
        {{ new Date().getFullYear() }} — <strong>Công Ty Bất Động Sản Tuấn Anh</strong>
        <br />
        <span>Số điện thoại: +84 123 456 789</span> | 
        <br />
        
        <span>Email: info@tuananhrealestate.com</span>
      </div>
    </v-footer>
  </template>
  
  <script>
  export default {
    data: () => ({
      icons: [
        'mdi-facebook',
        'mdi-twitter',
        'mdi-linkedin',
        'mdi-instagram',
      ],
    }),
  }
  </script>
  
  <style scoped>
  .bg-green-lighten-1 {
    background-color: #8BC34A !important; /* Màu xanh lá cây nhạt */
  }
  .bg-yellow-lighten-4 {
    background-color: #FFF59D !important; /* Màu vàng nhạt */
  }
  .text-white {
    color: #ffffff;
  }
  .v-btn {
    transition: all 0.3s ease;
  }
  .v-btn:hover {
    background-color: #FFEB3B !important; /* Đổi màu sang vàng khi hover */
  }
  </style>
  