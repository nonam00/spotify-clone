import { BrowserRouter, Routes, Route } from "react-router-dom";
import { ModeratePage } from "@/pages/moderate";
import { ConfirmModal } from "@/shared/ui";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<ModeratePage />} />
      </Routes>
      <ConfirmModal />
    </BrowserRouter>
  );
}

export default App;

