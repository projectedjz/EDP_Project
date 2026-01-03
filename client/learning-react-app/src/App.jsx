import './App.css';
import { useState, useEffect } from 'react';
import { Container, AppBar, Toolbar, Typography, Box, Button } from '@mui/material';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { ThemeProvider } from '@mui/material/styles';
import MyTheme from './themes/MyTheme';
import Tutorials from './pages/Tutorials';
import AddTutorial from './pages/AddTutorial';
import EditTutorial from './pages/EditTutorial';
import MyForm from './pages/MyForm';
import Register from './pages/Register';
import Login from './pages/Login';
import http from './http';
import UserContext from './contexts/UserContext';
import AdminNavbar from "./components/AdminNavBar";
import GBLNavBar from "./components/GBLNavBar";
import Inventory from './pages/Inventory';
import AddInventory from './pages/AddInventory';
import EditInventory from './pages/EditInventory';

function App() {
  const [user, setUser] = useState(null);

  useEffect(() => {
    if (localStorage.getItem("accessToken")) {
      http.get('/user/auth').then((res) => {
        setUser(res.data.user);
      });
    }
  }, []);

  const logout = () => {
    localStorage.clear();
    window.location = "/";
  };

  return (
    <UserContext.Provider value={{ user, setUser }}>
      <Router>
        <ThemeProvider theme={MyTheme}>
            <Container>
              <Toolbar disableGutters={true}>
                {/* Ziv add logic to check for admin using roles */}
                <AdminNavbar />
                {/* <GBLNavBar /> */}
              </Toolbar>
            </Container>

          <Container>
            <Routes>
              <Route path={"/"} element={<Tutorials />} />
              <Route path={"/tutorials"} element={<Tutorials />} />
              <Route path={"/addtutorial"} element={<AddTutorial />} />
              <Route path={"/edittutorial/:id"} element={<EditTutorial />} />


              <Route path={"/register"} element={<Register />} />
              <Route path={"/login"} element={<Login />} />
              <Route path={"/form"} element={<MyForm />} />

              <Route path={"/inventory"} element={<Inventory />} />
              <Route path={"/addinventory"} element={<AddInventory />} />
              <Route path={"/editinventory/:id"} element={<EditInventory />} />

            </Routes>
          </Container>
        </ThemeProvider>
      </Router>
    </UserContext.Provider>
  );
}

export default App;
