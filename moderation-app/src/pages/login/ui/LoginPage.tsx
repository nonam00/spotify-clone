import { type FormEvent, useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Box, Button, Input } from "@/shared/ui";
import { useAuthStore } from "@/features/auth";

const LoginPage = () => {
  const navigate = useNavigate();
  const { login, isLoading, error, isAuthenticated, checkAuth } = useAuthStore();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  useEffect(() => {
    if (!isAuthenticated) {
      void checkAuth();
    }
  }, [isAuthenticated, checkAuth]);

  useEffect(() => {
    if (isAuthenticated) {
      navigate("/", { replace: true });
    }
  }, [isAuthenticated, navigate]);

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const success = await login(email, password);
    if (success) {
      navigate("/", { replace: true });
    }
  };

  return (
    <div className="h-full flex items-center justify-center bg-black">
      <Box className="p-8 w-full max-w-md">
        <div className="flex flex-col gap-6">
          <div className="text-center">
            <h1 className="text-3xl font-bold text-white mb-2">Moderation Panel</h1>
            <p className="text-neutral-400">
              Use your moderator credentials to continue
            </p>
          </div>

          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            <div className="flex flex-col gap-y-2">
              <label className="text-white font-medium">Email:</label>
              <Input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="moderator@example.com"
                disabled={isLoading}
                required
                autoComplete="email"
              />
            </div>

            <div className="flex flex-col gap-y-2">
              <label className="text-white font-medium">Password:</label>
              <Input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="********"
                disabled={isLoading}
                required
                minLength={8}
                autoComplete="current-password"
              />
            </div>

            {error && (
              <div className="text-red-400 text-sm text-center bg-red-500/10 border border-red-500/30 rounded-md p-3">
                {error}
              </div>
            )}

            <Button
              type="submit"
              disabled={isLoading}
              className="bg-green-500 hover:bg-green-600 text-white mt-2"
            >
              {isLoading ? "Signing in..." : "Sign In"}
            </Button>
          </form>
        </div>
      </Box>
    </div>
  );
};

export default LoginPage;