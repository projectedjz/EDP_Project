import { useContext, useState } from "react";
import { useLocation, Link, useNavigate } from "react-router-dom";
import UserContext from "../contexts/UserContext";

import {
  AppBar,
  Toolbar,
  Typography,
  Box,
  Drawer,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  CssBaseline,
  Avatar,
  Button,
  Menu,
  MenuItem,
  Divider,
  IconButton,
  useTheme,
  useMediaQuery,
} from "@mui/material";

import MenuIcon from "@mui/icons-material/Menu";
import DashboardOutlinedIcon from "@mui/icons-material/DashboardOutlined";
import SchoolOutlinedIcon from "@mui/icons-material/SchoolOutlined";
import SettingsOutlinedIcon from "@mui/icons-material/SettingsOutlined";
import LogoutOutlinedIcon from "@mui/icons-material/LogoutOutlined";
import ArrowDropDownIcon from "@mui/icons-material/ArrowDropDown";

const drawerWidth = 260;

function Navbar() {
  const { user, setUser } = useContext(UserContext);
  const location = useLocation();
  const navigate = useNavigate();

  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("md"));

  const [mobileOpen, setMobileOpen] = useState(false);
  const [anchorEl, setAnchorEl] = useState(null);

  const menuOpen = Boolean(anchorEl);

  const navItems = [
    { label: "Dashboard", path: "/", icon: <DashboardOutlinedIcon /> },
    { label: "Tutorials", path: "/tutorials", icon: <DashboardOutlinedIcon /> },
  ];

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const handleMenuOpen = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleLogout = () => {
    localStorage.clear();
    setUser(null);
    handleMenuClose();
    navigate("/login");
  };

  /* Sidebar Content */
  const drawerContent = (
    <List sx={{ px: 1 }}>
      {navItems.map((item) => {
        const isActive = location.pathname === item.path;

        return (
          <ListItemButton
            key={item.label}
            component={Link}
            to={item.path}
            onClick={() => isMobile && setMobileOpen(false)}
            sx={{
              mb: 0.5,
              borderRadius: 2,
              backgroundColor: isActive ? "#e7f7ed" : "transparent",
              color: isActive ? "#16a34a" : "#374151",
              "&:hover": { backgroundColor: "#f0fdf4" },
            }}
          >
            <ListItemIcon
              sx={{
                minWidth: 36,
                color: isActive ? "#16a34a" : "#6b7280",
              }}
            >
              {item.icon}
            </ListItemIcon>
            <ListItemText
              primary={item.label}
              primaryTypographyProps={{
                fontSize: 14,
                fontWeight: isActive ? 600 : 500,
              }}
            />
          </ListItemButton>
        );
      })}
    </List>
  );

  return (
    <Box sx={{ display: "flex" }}>
      <CssBaseline />

      {/* Top Navbar */}
      <AppBar
        position="fixed"
        elevation={0}
        sx={{
          zIndex: (theme) => theme.zIndex.drawer + 1,
          backgroundColor: "#ffffff",
          borderBottom: "1px solid #e5e7eb",
          color: "#111827",
          boxShadow: "0 4px 6px rgba(0,0,0,0.1)",
        }}
      >
        <Toolbar sx={{ justifyContent: "space-between" }}>
          {/* Hamburger */}
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            {isMobile && (
              <IconButton onClick={handleDrawerToggle} edge="start">
                <MenuIcon />
              </IconButton>
            )}

            <Typography
              sx={{ color: "#16a34a", fontWeight: 700, fontSize: 20 }}
            >
              Meod
            </Typography>
            <Typography variant="body2" sx={{ color: "#6b7280" }}>
              | Admin Portal
            </Typography>
          </Box>

          {/* Login and Register */}
          {!user ? (
            <Box sx={{ display: "flex", gap: 1 }}>
              <Button
                component={Link}
                to="/login"
                variant="outlined"
                size="small"
              >
                Login
              </Button>
              <Button
                component={Link}
                to="/register"
                variant="contained"
                size="small"
                sx={{ backgroundColor: "#16a34a" }}
              >
                Register
              </Button>
            </Box>
          ) : (
            <>
              <Button
                onClick={handleMenuOpen}
                sx={{
                  textTransform: "none",
                  display: "flex",
                  alignItems: "center",
                  gap: 1,
                  color: "inherit",
                }}
              >
                <Avatar sx={{ bgcolor: "#16a34a", width: 32, height: 32 }}>
                  {user.name?.charAt(0)}
                </Avatar>
                {!isMobile && (
                  <Box sx={{ textAlign: "left" }}>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      {user.name}
                    </Typography>
                    <Typography variant="caption" sx={{ color: "#6b7280" }}>
                      {user.role || "Staff Member"}
                    </Typography>
                  </Box>
                )}
                <ArrowDropDownIcon />
              </Button>

              <Menu
                anchorEl={anchorEl}
                open={menuOpen}
                onClose={handleMenuClose}
              >
                <MenuItem
                  onClick={() => {
                    handleMenuClose();
                    navigate("/settings");
                  }}
                >
                  <SettingsOutlinedIcon fontSize="small" sx={{ mr: 1 }} />
                  Settings
                </MenuItem>

                <Divider />

                <MenuItem onClick={handleLogout} sx={{ color: "error.main" }}>
                  <LogoutOutlinedIcon fontSize="small" sx={{ mr: 1 }} />
                  Logout
                </MenuItem>
              </Menu>
            </>
          )}
        </Toolbar>
      </AppBar>

      {/* Side Navbar */}
      <Drawer
        variant={isMobile ? "temporary" : "permanent"}
        open={isMobile ? mobileOpen : true}
        onClose={handleDrawerToggle}
        ModalProps={{ keepMounted: true }}
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          "& .MuiDrawer-paper": {
            width: drawerWidth,
            boxSizing: "border-box",
            marginTop: "64px",
            borderRight: "1px solid #e5e7eb",
          },
        }}
      >
        {drawerContent}
      </Drawer>
    </Box>
  );
}

export default Navbar;
