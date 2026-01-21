import { useEffect, type ReactNode } from "react";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { routes } from "./routes";
import { useAuth } from "./hooks";
import { Spin } from "antd";
import "./App.css";

interface ProtectedRouteProps {
  children: ReactNode;
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const { account, isLoading, fetchInfo } = useAuth();

  useEffect(() => {
    const fetchAccount = async () => {
      await fetchInfo();

      if (!account) {
        return <Navigate to="/login" replace />;
      }
    };

    fetchAccount();
  }, []);

  return isLoading ? <Spin fullscreen /> : <>{children}</>;
};

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {routes.map((route) => {
          const Component = route.component;
          return (
            <Route
              key={route.path}
              path={route.path}
              element={
                route.isProtected ? (
                  <ProtectedRoute>
                    <Component />
                  </ProtectedRoute>
                ) : (
                  <Component />
                )
              }
            />
          );
        })}
        <Route path="*" element={<Navigate to="/home" />} />
      </Routes>
    </BrowserRouter>
  );
}
