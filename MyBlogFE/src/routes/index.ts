import {
  HomePage,
  LoginPage,
  MyProfilePage,
  ProfilePage,
  ViewPostPage,
} from "@/pages";
import type { ComponentType } from "react";

interface RouteConfig {
  /** The url of the route */
  path: string;

  /** The React component to render for this route */
  component: ComponentType;

  /** Whether the route is protected and requires authentication */
  isProtected: boolean;
}

export const routes: RouteConfig[] = [
  { path: "/login", component: LoginPage, isProtected: false },
  { path: "/home", component: HomePage, isProtected: true },
  { path: "/", component: HomePage, isProtected: true },
  { path: "/post/", component: ViewPostPage, isProtected: true },
  { path: "/profile", component: ProfilePage, isProtected: true },
  { path: "/profile/me", component: MyProfilePage, isProtected: true },
];

//   { path: "/", component: HomePage, protected: true },
//   { path: "/confirm", component: ConfirmPage, protected: false },
//   { path: "/notFound", component: NotFoundPage, protected: false },
//   { path: "/home", component: HomePage, protected: true },
//   { path: "/post", component: ViewPostPage, protected: true },
//   { path: "/profile/me", component: MyProfilePage, protected: true },
//   { path: "/profile", component: ProfilePage, protected: true },
//   { path: "/account/search", component: AccountSearchPage, protected: true },
