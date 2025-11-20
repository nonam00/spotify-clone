import { memo } from "react";
import * as RadixSlider from "@radix-ui/react-slider";

const Slider = memo(function Slider({
  value = 0,
  onChange,
  max = 1,
  step = 0.01
}: {
  value: number;
  onChange: (value: number[]) => void;
  max?: number;
  step?: number;
}) {
  return (
    <RadixSlider.Root
      className="relative flex items-center select-none touch-none w-full h-7 cursor-pointer"
      value={[value]}
      onValueChange={onChange}
      max={max}
      step={step}
      aria-label="Slider"
    >
      <RadixSlider.Track className="bg-neutral-600 relative grow rounded-full h-[3.1px]">
        <RadixSlider.Range className="absolute bg-white rounded-full h-full"/>
      </RadixSlider.Track>
      <RadixSlider.Thumb className="block w-3 h-3 bg-white rounded-full shadow-sm hover:shadow-md focus:outline-none focus:shadow-lg transition-all" />
    </RadixSlider.Root>
  );
});

export default Slider;

