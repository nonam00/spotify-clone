import { forwardRef } from "react";
import { twMerge } from "tailwind-merge";

type InputProps = React.InputHTMLAttributes<HTMLInputElement>;

const Input = forwardRef<HTMLInputElement, InputProps>(({
  className,
  type,
  disabled,
  ...props
}, ref) => {
  return (
    <input
      type={type}
      className={twMerge(`
        flex w-full px-3 py-3
        rounded-md border border-transparent bg-neutral-700 text-sm text-white
        placeholder:text-neutral-400
        disabled:cursor-not-allowed disabled:opacity-50 
        focus:outline-none focus:ring-2 focus:ring-green-500 focus:border-transparent
        transition-all
      `,
        className
      )}
      disabled={disabled}
      ref={ref}
      {...props}
    />
  );
});

Input.displayName = 'Input';

export default Input;