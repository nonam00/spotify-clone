"use client";

import { useRouter } from "next/navigation";
import { useCallback, useTransition } from "react";
import { useShallow } from "zustand/shallow";

import { Button, Modal } from "@/shared/ui";
import { useConfirmModalStore } from "../model";

const ConfirmModal = () => {
  const router = useRouter();

  const { onClose, isOpen, action, description } = useConfirmModalStore(
    useShallow((s) => ({
      onClose: s.onClose,
      isOpen: s.isOpen,
      action: s.action,
      description: s.description,
    }))
  );

  const [isPending, startTransition] = useTransition();

  const onChange = useCallback((open: boolean) => {
    if (!open) {
      onClose();
    }
  }, [onClose]);

  const onConfirmClick = async () => {
    startTransition(async () => {
      await action();
      onClose();
      router.refresh();
    });
  };

  return (
    <Modal
      title="Are you sure you want to do this?"
      description={description}
      onChange={onChange}
      isOpen={isOpen}
    >
      <div className="flex flex-row py-4">
        <Button
          onClick={onClose}
          disabled={isPending}
          className="text-white bg-transparent"
        >
          Cancel
        </Button>
        <Button onClick={onConfirmClick} disabled={isPending}>
          Confirm
        </Button>
      </div>
    </Modal>
  );
};

export default ConfirmModal;