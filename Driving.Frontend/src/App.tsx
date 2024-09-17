import { useEffect, useState } from "react";
import "./App.css";
import { Button } from "./components/ui/button";
import { tagApi } from "./api-configs";
import { Tag } from "./api";
import { Input } from "./components/ui/input";

function App() {
  const [data, setData] = useState<Tag[] | null>(null);
  const [loading, setLoading] = useState(true);
  const [inputValue, setInputValue] = useState("");

  async function fetchData() {
    setLoading(true);
    setData(null);
    try {
      const tags = await tagApi.tagAllGet();
      setData(tags.data);
    } catch (error) {
      alert(error);
    } finally {
      setLoading(false);
    }
  }
  async function addTag() {
    try {
      await tagApi.tagCreatePost({ name: inputValue });
      fetchData();
    } catch (error) {
      alert(error);
    }
  }

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <>
      <div>
        <div onClick={() => fetchData()}>
          {data ? (
            <pre>{JSON.stringify(data, null, 2)}</pre>
          ) : loading ? (
            "Loading..."
          ) : (
            "Fetch Data"
          )}
        </div>
      </div>
      <div className="flex w-full max-w-sm items-center space-x-2">
        {inputValue}
        <Input
          value={inputValue}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setInputValue(e.target.value)
          }
        />
        <Button onClick={addTag}>add tag</Button>
      </div>
    </>
  );
}

export default App;
