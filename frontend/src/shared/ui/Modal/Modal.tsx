import * as Dialog from "@radix-ui/react-dialog";
import { IoMdClose } from "react-icons/io";
import { twMerge } from "tailwind-merge";

type ModalProps = {
  isOpen: boolean;
  onChange: (open: boolean) => void;
  title: string;
  description: string;
  children: React.ReactNode;
  className?: string;
  overlayClassName?: string
};

export function Modal({isOpen, onChange, title, description, children, className, overlayClassName }: ModalProps) {
  return (
    <Dialog.Root open={isOpen} defaultOpen={isOpen} onOpenChange={onChange}>
      <Dialog.Portal>
        <Dialog.Overlay
          className={twMerge(`bg-black/70 backdrop-blur-md fixed inset-0`, overlayClassName)} />
        <Dialog.Content
          className={twMerge(`
            fixed
            drop-shadow-md
            border border-neutral-700
            top-[50%] left-[50%]
            max-h-full h-full
            md:h-auto md:max-h-[85vh]
            w-full
            md:w-[450px] md:max-w-[90vw]
            translate-x-[-50%] translate-y-[-50%]
            rounded-md
            bg-neutral-900
            p-[25px]
            focus:outline-none
          `, className)
          }
        >
          <Dialog.Title className="text-xl text-center font-bold mb-4">
            {title}
          </Dialog.Title>
          <Dialog.Description className="mb-5 text-sm leading-normal text-center">
            {description}
          </Dialog.Description>
          <div>{children}</div>
          <Dialog.Close asChild>
            <button
              className="
                absolute
                inline-flex
                items-center
                justify-center
                top-[10px] right-[10px]
                h-[25px] w-[25px]
                appearance-none
                rounded-full
                text-neutral-400
                hover:text-white
                focus:outline-none
              "
            >
              <IoMdClose />
            </button>
          </Dialog.Close>
        </Dialog.Content>
      </Dialog.Portal>
    </Dialog.Root>
  );
}