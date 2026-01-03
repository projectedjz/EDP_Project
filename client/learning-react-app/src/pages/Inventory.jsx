import React, { useEffect, useState, useMemo, useRef, useContext } from "react";
import { Link } from "react-router-dom";
import {
  Box,
  TextField,
  IconButton,
  Collapse,
  InputAdornment,
  Tooltip,
  Menu,
  MenuItem,
  Checkbox,
  Button,
  Grow,
  Typography,
} from "@mui/material";
import {
  Search,
  FilterList,
  Download,
  Close,
  Add,
  Delete,
  Create,
} from "@mui/icons-material";
import { DataGrid } from "@mui/x-data-grid";
import http from "../http";
import ConfirmDelete from "../components/ConfirmDelete";
import { toast } from "react-toastify";
import UserContext from "../contexts/UserContext";
import dayjs from "dayjs";

function Inventory() {
  const { user } = useContext(UserContext);

  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [searchOpen, setSearchOpen] = useState(false);
  const [openFilterMenu, setOpenFilterMenu] = useState(null);
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [selectedRows, setSelectedRows] = useState([]);

  const allColumns = useMemo(
    () => [
      { field: "InventoryId", headerName: "ID", flex: 0.5 },
      { field: "ProductName", headerName: "Product", flex: 1 },
      { field: "Quantity", headerName: "Quantity (kg)", flex: 1 },
      { field: "HarvestDate", headerName: "Harvest Date", flex: 1 },
      { field: "ExpiryDate", headerName: "Expiry Date", flex: 1 },
      {
        field: "ExpiryStatus",
        headerName: "Expiry Status",
        flex: 1,
        renderCell: (params) => {
          const { createdAt, expiryDate } = params.row;
          const status = getExpiryStatus(createdAt, expiryDate);

          return (
            <Box
              sx={{
                display: "inline-flex",
                alignItems: "center",
                justifyContent: "center",
                minWidth: 90,
                height: 28,
                px: 2,
                borderRadius: "999px",
                backgroundColor: status.bg,
                color: status.color,
                fontWeight: 600,
                fontSize: "0.8rem",
                boxShadow: "inset 0 0 0 1px rgba(0,0,0,0.08)",
                lineHeight: 1,
                whiteSpace: "nowrap",
              }}
            >
              {status.label}
            </Box>
          );
        },
      },
      { field: "LocationName", headerName: "Location", flex: 1 },
    ],
    []
  );

  const [columnVisibility, setColumnVisibility] = useState({
    InventoryId: true,
    ProductName: true,
    Quantity: true,
    HarvestDate: true,
    ExpiryDate: true,
    ExpiryStatus: true,
    LocationName: true,
  });

  const visibleColumns = useMemo(
    () => allColumns.filter((col) => columnVisibility[col.field]),
    [columnVisibility]
  );

  const fetchInventory = () => {
    setLoading(true);
    const endpoint = search ? `/inventory?search=${search}` : "/inventory";
    http
      .get(endpoint)
      .then((res) => {
        const data = res.data
          .filter((item) => item.inventoryId)
          .map((item) => ({
            InventoryId: item.inventoryId,
            ProductName: item.productName || "",
            Quantity: item.quantity,
            HarvestDate: dayjs(item.harvestDate).format("DD/MM/YYYY"),
            ExpiryDate: dayjs(item.expiryDate).format("DD/MM/YYYY"),
            LocationName: item.locationName || "",
            createdAt: item.createdAt,
            expiryDate: item.expiryDate,
          }));
        setRows(data);
      })
      .catch((err) => console.error("Error fetching inventory:", err))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchInventory();
  }, []);

  const onSearchKeyDown = (e) => {
    if (e.key === "Enter") fetchInventory();
  };
  const handleFilterClick = (e) => setOpenFilterMenu(e.currentTarget);
  const handleFilterClose = () => setOpenFilterMenu(null);
  const toggleColumn = (field) =>
    setColumnVisibility((prev) => ({ ...prev, [field]: !prev[field] }));
  const handleDeleteClick = () => setConfirmOpen(true);

  const handleConfirmDelete = async () => {
    if (selectedRows.length === 0) return;
    try {
      await Promise.all(
        selectedRows.map((id) => http.delete(`/inventory`, { data: [id] }))
      );
      fetchInventory();
      setSelectedRows([]);
      toast.success("Inventory items deleted successfully.");
    } catch (err) {
      console.error("Failed to delete items:", err);
      toast.error("Failed to delete items.");
    }
    setConfirmOpen(false);
  };

  const handleCancelDelete = () => setConfirmOpen(false);

  const handleDownloadCSV = () => {
    const csvHeader = visibleColumns.map((col) => col.headerName).join(",");
    const csvRows = rows.map((row) =>
      visibleColumns
        .map((col) => {
          if (col.field === "ExpiryStatus") {
            return `"${getExpiryStatus(row.createdAt, row.expiryDate).label}"`;
          }
          return `"${row[col.field]}"`;
        })
        .join(",")
    );
    
    const csvContent = [csvHeader, ...csvRows].join("\n");
    const blob = new Blob([csvContent], { type: "text/csv" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "inventory.csv";
    a.click();
    URL.revokeObjectURL(url);
  };

  const searchRef = useRef();
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (searchRef.current && !searchRef.current.contains(event.target))
        setSearchOpen(false);
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const getExpiryStatus = (createdAt, expiryDate) => {
    const now = new Date();
    const expiry = new Date(expiryDate);

    const remainingMs = expiry - now;

    if (remainingMs <= 0) {
      return {
        label: "Expired",
        bg: "#ef4444",
        color: "#fff",
      };
    }

    const remainingHours = Math.floor(remainingMs / (1000 * 60 * 60));
    const remainingDays = Math.floor(remainingHours / 24);

    let bg = "#22c55e";
    if (remainingHours <= 72) bg = "#facc15";
    if (remainingHours <= 24) bg = "#ef4444";

    return {
      label:
        remainingDays > 0
          ? `${remainingDays}d ${remainingHours % 24}h`
          : `${remainingHours}h`,
      bg,
      color: remainingHours <= 24 ? "#fff" : "#000",
    };
  };

  return (
    <Box sx={{ width: "100%" }}>
      <Typography variant="h5" sx={{ my: 2 }}>
        Inventory
      </Typography>

      <Box sx={{ display: "flex", alignItems: "center", gap: 1, mb: 2 }}>
        <Box ref={searchRef} sx={{ display: "flex", alignItems: "center" }}>
          <Collapse in={searchOpen} orientation="horizontal">
            <TextField
              size="small"
              placeholder="Search inventory..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              onKeyDown={onSearchKeyDown}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={() => setSearchOpen(false)}>
                      <Close />
                    </IconButton>
                  </InputAdornment>
                ),
              }}
              sx={{ width: 250 }}
            />
          </Collapse>
          {!searchOpen && (
            <Grow in={!searchOpen}>
              <Tooltip title="Search">
                <IconButton onClick={() => setSearchOpen(true)}>
                  <Search />
                </IconButton>
              </Tooltip>
            </Grow>
          )}
        </Box>

        <Tooltip title="Toggle Columns">
          <IconButton onClick={handleFilterClick}>
            <FilterList />
          </IconButton>
        </Tooltip>
        <Menu
          anchorEl={openFilterMenu}
          open={Boolean(openFilterMenu)}
          onClose={handleFilterClose}
        >
          {allColumns.map((col) => (
            <MenuItem key={col.field}>
              <Checkbox
                checked={columnVisibility[col.field]}
                onChange={() => toggleColumn(col.field)}
              />
              {col.headerName}
            </MenuItem>
          ))}
        </Menu>

        <Tooltip title="Download CSV">
          <IconButton onClick={handleDownloadCSV}>
            <Download />
          </IconButton>
        </Tooltip>

        {selectedRows.length === 1 && (
          <Tooltip title="Update Row">
            <Link to={`/editinventory/${selectedRows[0]}`}>
              <IconButton>
                <Create />
              </IconButton>
            </Link>
          </Tooltip>
        )}

        {selectedRows.length > 0 && (
          <Tooltip title="Delete Row">
            <IconButton onClick={handleDeleteClick}>
              <Delete />
            </IconButton>
          </Tooltip>
        )}

        <ConfirmDelete
          open={confirmOpen}
          onClose={handleCancelDelete}
          onConfirm={handleConfirmDelete}
          itemName={
            selectedRows.length > 1
              ? `${selectedRows.length} items`
              : `ID ${selectedRows[0]}`
          }
        />

        <Box sx={{ ml: "auto" }}>
          <Link to="/addinventory">
            <Button variant="contained" startIcon={<Add />}>
              Add Item
            </Button>
          </Link>
        </Box>
      </Box>

      <DataGrid
        getRowId={(row) => row.InventoryId}
        initialState={{
          pagination: { paginationModel: { pageSize: 25, page: 0 } },
        }}
        rows={rows}
        columns={allColumns}
        loading={loading}
        checkboxSelection
        onRowSelectionModelChange={(newSelection) =>
          setSelectedRows(Array.from(newSelection.ids))
        }
        disableRowSelectionOnClick
        columnVisibilityModel={columnVisibility}
        onColumnVisibilityModelChange={(newModel) =>
          setColumnVisibility(newModel)
        }
      />
    </Box>
  );
}

export default Inventory;
