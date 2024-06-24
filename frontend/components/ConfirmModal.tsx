"use client";

import { useRouter } from "next/navigation";

import useConfirmModal from "@/hooks/useConfirmModal";

import Modal from "./Modal";
import Button from "./Button";

const ConfirmModal = () => {
  const router = useRouter();
  const { onClose, isOpen, action, description } = useConfirmModal();
  
  const onChange = (open: boolean) => {
    if (!open) {
      onClose();
    } 
  }

  const onConfirmClick = async () => {
    await action();
    onClose();
    router.refresh();
  }

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
          className="
            text-white
            bg-transparent
          "
        >
          Cancel 
        </Button>
        <Button
          onClick={onConfirmClick}
        >
          Confirm
        </Button>
      </div>
    </Modal>
  )
}

export default ConfirmModal;
