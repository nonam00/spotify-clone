import { useState, type SubmitEvent } from "react";
import {useShallow} from "zustand/react/shallow";
import { z } from 'zod';

import { Box, Button, Input } from "@/shared/ui";
import { useModeratorsStore } from "../model";

const registerModeratorSchema = z.object({
  email: z.email("Invalid email address")
    .trim()
    .min(1, "Email is required")
    .max(255, "Email must be less than 255 characters"),
  password: z.string()
    .trim()
    .min(8, "Password must be at least 8 characters")
    .max(100, "Password must be less than 100 characters"),
  fullName: z.string()
    .trim()
    .min(1, "Full name is required")
    .max(255, "Full name must be less than 100 characters"),
  isSuper: z.boolean(),
});

type RegisterModeratorFormData = z.infer<typeof registerModeratorSchema>;

const initialFormData: RegisterModeratorFormData = {
  email: "",
  password: "",
  fullName: "",
  isSuper: false,
}

const CreateModeratorForm = () => {
  const { createModerator, isLoading } = useModeratorsStore(
    useShallow((s) => ({
      createModerator: s.createModerator,
      isLoading: s.isLoading,
    }))
  );

  const [formData, setFormData] = useState<RegisterModeratorFormData>({...initialFormData});

  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [requestError, setRequestError] = useState<string | null>(null);
  const [showErrors, setShowErrors] = useState<boolean>(false);

  const validate = () => {
    const result = registerModeratorSchema.safeParse(formData);
    if (result.success) {
      return undefined;
    }
    return z.flattenError(result.error);
  }

  const handleSubmit = async (event: SubmitEvent) => {
    event.preventDefault();
    setSuccessMessage(null);

    const errors = validate();
    if (errors) {
      setShowErrors(true)
      return;
    }

    try {
      await createModerator({...formData});
      setSuccessMessage("Moderator account created");
      setFormData({...initialFormData});
    } catch (err) {
      const message = err instanceof Error ? err.message : "Failed to create moderator";
      setRequestError(message);
    }
  };

  const errors = showErrors ? validate() + "; " + requestError : undefined;

  return (
    <Box className="p-6 space-y-6">
      <div>
        <h2 className="text-2xl font-semibold text-white">Create moderator account</h2>
        <p className="text-sm text-neutral-400 mt-1">Invite a teammate and configure access rights.</p>
      </div>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <label className="space-y-2">
            <span className="text-sm text-neutral-300">Email</span>
            <Input
              value={formData.email}
              onChange={(event) =>
                setFormData({...formData, email: event.target.value})
              }
              type="email"
              placeholder="Enter moderator email..."
              required
              maxLength={255}
            />
          </label>
          <label className="space-y-2">
            <span className="text-sm text-neutral-300">Full name</span>
            <Input
              value={formData.fullName}
              onChange={(event) =>
                setFormData({...formData, fullName: event.target.value})
              }
              type="text"
              placeholder="Enter moderator full name..."
              maxLength={255}
            />
          </label>
        </div>
        <label className="space-y-2 block">
          <span className="text-sm text-neutral-300">Password</span>
          <Input
            value={formData.password}
            onChange={(event) =>
              setFormData({...formData, password: event.target.value})
            }
            type="password"
            placeholder="Enter moderator password..."
            minLength={8}
            maxLength={100}
            required
          />
        </label>
        <label className="flex items-center gap-3 text-sm text-neutral-300 cursor-pointer select-none">
          <input
            type="checkbox"
            checked={formData.isSuper}
            onChange={(event) =>
              setFormData({...formData, isSuper: event.target.checked})
            }
            className="h-4 w-4 rounded border-neutral-600 bg-neutral-800 text-emerald-500 focus:ring-emerald-500"
          />
          Grant full access (super moderator)
        </label>
        {successMessage && <p className="text-sm text-emerald-400">{successMessage}</p>}
        {showErrors && <p className="text-sm text-red-400">{errors}</p>}
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