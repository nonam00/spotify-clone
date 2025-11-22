import { useState } from "react";
import { Modal, Button } from "@/shared/ui";
import { useConfirmModalStore } from "../model";

const ConfirmModal = () => {
  const { isOpen, title, description, onConfirm, onClose } = useConfirmModalStore();
  const [isLoading, setIsLoading] = useState(false);

  const handleConfirm = async () => {
    if (!onConfirm) return;
    
    setIsLoading(true);
    try {
      await onConfirm();
      onClose();
    } catch (error) {
      console.error("Error in confirm action:", error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onChange={(open) => !open && onClose()}
      title={title}
      description={description}
    >
      <div className="flex flex-row gap-3 justify-end pt-4">
        <Button
          onClick={onClose}
          disabled={isLoading}
          className="bg-neutral-700 hover:bg-neutral-600 text-white px-6 py-2.5 min-w-[100px]"
        >
          Cancel
        </Button>
        <Button
          onClick={handleConfirm}
          disabled={isLoading}
          className="bg-green-500 hover:bg-green-600 text-white px-6 py-2.5 min-w-[100px] disabled:opacity-50"
        >
          {isLoading ? "..." : "Confirm"}
        </Button>
      </div>
    </Modal>
  );
};

export default ConfirmModal;

