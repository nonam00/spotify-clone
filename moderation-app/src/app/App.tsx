import { BrowserRouter, Routes, Route } from "react-router-dom";

import { ModeratePage } from "@/pages/songs/ui";
import { LoginPage } from "@/pages/login/ui";
import { ModeratorsPage } from "@/pages/moderators/ui";
import { UsersPage } from "@/pages/users/ui";

import { ConfirmModal } from "@/shared/ui";
import { ProtectedRoute } from "@/features/auth/ui";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route
          path="/"
          element={
            <ProtectedRoute requiredPermission="canManageContent">
              <ModeratePage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/moderators"
          element={
            <ProtectedRoute requiredPermission="canManageModerators">
              <ModeratorsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/users"
          element={
            <ProtectedRoute requiredPermission="canManageUsers">
              <UsersPage />
            </ProtectedRoute>
          }
        />
      </Routes>
      <ConfirmModal />
    </BrowserRouter>
  );
}

export default App;

