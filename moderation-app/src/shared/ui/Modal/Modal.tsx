import * as Dialog from "@radix-ui/react-dialog";
import { IoMdClose } from "react-icons/io";
import { twMerge } from "tailwind-merge";

type ModalProps = {
  isOpen: boolean;
  onChange: (open: boolean) => void;
  title: string;
  description?: string;
  children: React.ReactNode;
  className?: string;
}

const Modal = ({
  isOpen,
  onChange,
  title,
  description,
  children,
  className
}: ModalProps) => {
  return (
    <Dialog.Root open={isOpen} onOpenChange={onChange}>
      <Dialog.Portal>
        <Dialog.Overlay className="bg-black/60 backdrop-blur-sm fixed inset-0 z-40 data-[state=open]:animate-overlayShow data-[state=closed]:animate-overlayHide" />
        <Dialog.Content
          className={twMerge(`
            fixed
            drop-shadow-2xl
            border border-neutral-700/50
            top-[50%] left-[50%]
            max-h-full h-full
            md:h-auto md:max-h-[85vh]
            w-full
            md:w-[90vw] md:max-w-[500px]
            -translate-x-1/2 -translate-y-1/2
            rounded-xl
            bg-gradient-to-br from-neutral-900 via-neutral-800 to-neutral-900
            p-8
            focus:outline-none
            z-50
            data-[state=open]:animate-contentShow
            data-[state=closed]:animate-contentHide
          `, className)}
        >
          <Dialog.Title className="text-2xl font-bold mb-2 text-white">
            {title}
          </Dialog.Title>
          {description && (
            <Dialog.Description className="mb-6 text-sm leading-relaxed text-neutral-300">
              {description}
            </Dialog.Description>
          )}
          <div>
            {children}
          </div>
          <Dialog.Close asChild>
            <button
              className="
                absolute
                inline-flex
                items-center
                justify-center
                top-4 right-4
                h-8 w-8
                appearance-none
                rounded-full
                text-neutral-400
                hover:text-white
                hover:bg-neutral-700/50
                focus:outline-none
                transition-colors
              "
              aria-label="Close"
            >
              <IoMdClose size={20} />
            </button>
          </Dialog.Close>
        </Dialog.Content>
      </Dialog.Portal>
    </Dialog.Root>
  );
};

export default Modal;


