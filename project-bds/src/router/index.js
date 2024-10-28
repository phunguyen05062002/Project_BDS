import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import LoginForm from "@/components/LoginForm.vue";
import RegisterForm from "@/components/RegisterForm.vue";
import HomePage from "@/components/HomePage.vue";
import Home from "@/components/Home.vue";
import QuanLyBDS from "@/components/QuanLyBDS/QLThongTin.vue";
import QuanLyTK from "@/components/QuanLyTK.vue";
import Account from "@/components/Profiles/Account.vue";
import ChangePassword from "@/components/Profiles/ChangePassword.vue";
import Contact from "@/components/Contact.vue";
import QLQuyen from "@/components/Decentralizations/QLQuyen.vue";
import CapNhatQuyen from "@/components/Decentralizations/CapNhatQuyen.vue";
import QLAnh from "@/components/QuanLyBDS/QLAnh.vue";
import GuiLichHen from "@/components/GuiLichHen.vue";
import QLLoaiBDS from "@/components/QuanLyBDS/QLLoaiBDS.vue";
import ThongKe from "@/components/ThongKe.vue";
import ForgotPassword from "@/components/Password/ForgotPassword.vue";
import ResetPassword from "@/components/Password/ResetPassword.vue";

const routes = [
  {
    path: "/",
    name: "home",
    component: HomeView,
  },
  {
    path: "/login",
    name: "Login",
    component: LoginForm,
  },
  {
    path: "/forgot-password",
    name: "ForgotPassword",
    component: ForgotPassword,
  },
  {
    path: "/reset-password",
    name: "ResetPassword",
    component: ResetPassword,
  },

  {
    path: "/register",
    name: "Register",
    component: RegisterForm,
  },
  {
    path: "/home-page",
    name: "HomePage",
    component: HomePage,
  },
  {
    path: "/trang-chu",
    name: "TrangChu",
    component: Home,
  },
  {
    path: "/phan-quyen/quan-ly-quyen",
    name: "QLQuyen",
    component: QLQuyen,
  },
  {
    path: "/phan-quyen/cap-nhat-quyen",
    name: "CapNhatQuyen",
    component: CapNhatQuyen,
  },
  {
    path: "/quan-ly-BDS/quan-ly-thong-tin",
    name: "QuanLyBDS",
    component: QuanLyBDS,
  },
  {
    path: "/quan-ly-BDS/quan-ly-loai-BDS",
    name: "QuanLyLoaiBDS",
    component: QLLoaiBDS,
  },
  {
    path: "/quan-ly-BDS/quan-ly-anh",
    name: "QuanLyAnh",
    component: QLAnh,
  },
  {
    path: "/quan-ly-TK",
    name: "QuanLyTk",
    component: QuanLyTK,
  },

  {
    path: "/ca-nhan/tai-khoan",
    name: "TaiKhoan",
    component: Account,
  },
  {
    path: "/ca-nhan/doi-mat-khau",
    name: "DoiMatKhau",
    component: ChangePassword,
  },
  {
    path: "/lien-he",
    name: "LienHe",
    component: Contact,
  },
  {
    path: "/gui-lich-hen",
    name: "GuiLichHen",
    component: GuiLichHen,
  },
  {
    path: "/thong-ke",
    name: "ThongKe",
    component: ThongKe,
  },
  {
    path: "/about",
    name: "about",
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/AboutView.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
