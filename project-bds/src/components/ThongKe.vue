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
              <v-icon large class="mr-2">mdi-chart-bar</v-icon>
              <span>Thống kê bất động sản</span>
            </v-card-title>
          </v-card>
        </v-col>
      </v-card>
      <v-card class="pa-4 mb-4">
        <v-col cols="12" class="chart-container">
          <bar-chart
            ref="chart"
            :chart-data="chartData"
            :options="chartOptions"
          ></bar-chart>
        </v-col>
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
import HomePage from "./HomePage.vue";
import axios from "axios";
import { Bar } from "vue-chartjs";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
} from "chart.js";

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale
);

export default {
  components: {
    HomePage,
    BarChart: Bar,
  },
  data() {
    return {
      chartData: {
        labels: [],
        datasets: [
          {
            label: "Tỷ lệ phần trăm theo loại",
            backgroundColor: "#f87979",
            data: [],
          },
        ],
      },
      chartOptions: {
        responsive: true,
        maintainAspectRatio: false,
        aspectRatio: 0.5, // Tỷ lệ khung hình mới để biểu đồ không quá cao
        layout: {
          padding: {
            left: 10,
            right: 10,
            top: 10,
            bottom: 20,
          },
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              callback: function (value) {
                return value + "%";
              },
              stepSize: 2, // Điều chỉnh kích thước bước để giảm kích thước của nhãn trục y
            },
            min: 0,
            max: 100,
          },
        },
      },
    };
  },

  async mounted() {
    try {
      const accessToken = localStorage.getItem("accessToken");
      const response = await axios.get(
        "https://localhost:7067/api/Product/GetTotalPriceByType/GetTotalPriceByType",
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        }
      );

      const data = response.data;
      const total = Object.values(data).reduce((acc, value) => acc + value, 0);

      this.chartData.labels = Object.keys(data).map((key) => `Loại ${key}`);
      this.chartData.datasets[0].data = Object.values(data).map((value) =>
        ((value / total) * 100).toFixed(2)
      );

      this.$nextTick(() => {
        const chart = this.$refs.chart.$data._chart;
        chart.resize();
      });

      window.addEventListener('resize', () => {
        if (this.$refs.chart && this.$refs.chart.$data._chart) {
          this.$refs.chart.$data._chart.resize();
        }
      });
    } catch (error) {
      console.error("Failed to fetch data", error);
    }
  },
};
</script>

<style scoped>
.content {
  padding: 20px;
}
.title-card {
  background-color: #f5f5f5;
  border-radius: 8px;
}
.chart-container {
  height: 100%; /* Điều chỉnh chiều cao của phần chứa biểu đồ */
  position: relative;
}
</style>
