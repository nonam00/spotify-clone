import { useState, type FormEvent } from "react";
import { useModeratorsStore } from "@/features/manage-moderators/model/store";
import { Box, Button, Input } from "@/shared/ui";

const CreateModeratorForm = () => {
  const { createModerator, isLoading } = useModeratorsStore();
  const [email, setEmail] = useState("");
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");
  const [isSuper, setIsSuper] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setMessage(null);
    setError(null);

    if (!email || !password) {
      setError("Email and password are required");
      return;
    }

    try {
      await createModerator({
        email,
        fullName,
        password,
        isSuper,
      });
      setMessage("Moderator account created");
      setEmail("");
      setFullName("");
      setPassword("");
      setIsSuper(false);
    } catch (err) {
      const message = err instanceof Error ? err.message : "Failed to create moderator";
      setError(message);
    }
  };

  return (
    <Box className="p-6 space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-white">Add moderator</h2>
        <p className="text-sm text-neutral-400 mt-1">Invite a teammate and configure access rights.</p>
      </div>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <label className="space-y-2">
            <span className="text-sm text-neutral-300">Email</span>
            <Input
              type="email"
              placeholder="moderator@example.com"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              required
            />
          </label>
          <label className="space-y-2">
            <span className="text-sm text-neutral-300">Full name</span>
            <Input
              type="text"
              placeholder="Full name"
              value={fullName}
              onChange={(event) => setFullName(event.target.value)}
            />
          </label>
        </div>
        <label className="space-y-2 block">
          <span className="text-sm text-neutral-300">Temporary password</span>
          <Input
            type="password"
            placeholder="********"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
          />
        </label>
        <label className="flex items-center gap-3 text-sm text-neutral-300 cursor-pointer select-none">
          <input
            type="checkbox"
            checked={isSuper}
            onChange={(event) => setIsSuper(event.target.checked)}
            className="h-4 w-4 rounded border-neutral-600 bg-neutral-800 text-emerald-500 focus:ring-emerald-500"
          />
          Grant full access (super moderator)
        </label>
        {message && <p className="text-sm text-emerald-400">{message}</p>}
        {error && <p className="text-sm text-red-400">{error}</p>}
        <div className="flex justify-end">
          <Button
            type="submit"
            disabled={isLoading}
            className="bg-emerald-500 hover:bg-emerald-600 text-white px-6 py-3 disabled:opacity-60"
          >
            Invite moderator
          </Button>
        </div>
      </form>
    </Box>
  );
};

export default CreateModeratorForm;

