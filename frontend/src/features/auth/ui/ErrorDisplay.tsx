import {MdError} from "react-icons/md";

type ErrorDisplayProps = {
  error: string | null;
  className?: string;
}

const ErrorDisplay = ({ error, className = "" }: ErrorDisplayProps) => {
  if (!error) {
    return null;
  }

  return (
    <div
      className={`
        animate-fadeIn
        w-full text-red-400 text-sm 
        bg-red-500/10 border border-red-500/30 
        rounded-md py-2 px-3
        flex items-center gap-2
        ${className}
      `}
    >
      <MdError size={18} className="text-red-400 hover:text-red-500/10" />
      <span>{error}</span>
    </div>
  );
};

export default ErrorDisplay;