"use client";

import * as RadixSlider from "@radix-ui/react-slider";
import {memo} from "react";

const Slider = memo(function Slider({
  value = 1,
  onChange
}: {
  value: number;
  onChange: (value: number[]) => void;
}) {
  return (
    <RadixSlider.Root
      className="relative flex items-center select-none touch-none w-full h-7"
      defaultValue={[1]}
      value={[value]}
      onValueChange={onChange}
      max={1}
      step={0.01}
      aria-label="Volume"
    >
      <RadixSlider.Track className="bg-neutral-600 relative grow rounded-full h-[3.1px] cursor-pointer">
        <RadixSlider.Range className="absolute bg-white rounded-full h-full"/>
      </RadixSlider.Track>
    </RadixSlider.Root>
  );
});
 
export default Slider;