import { BrowserRouter, Routes, Route } from "react-router-dom";

import { ModeratePage } from "@/pages/songs";
import { LoginPage } from "@/pages/login";
import { ModeratorsPage } from "@/pages/moderators";
import { UsersPage } from "@/pages/users";

import { ProtectedRoute } from "@/shared/components/ProtectedRoute";
import { ConfirmModal } from "@/shared/ui";

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

