import { Navbar, Nav, Container} from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';

const NavBarComponent = () => {
  return (
    <Navbar className="bg-egyedi border-egyedi mb-3" expand="lg">
      <Container>
        <Navbar.Brand className="color">Todo</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="mr-auto">
            <LinkContainer to="/" exact>
              <Nav.Link className="nav-bar-link-color">Feladatok</Nav.Link>
            </LinkContainer>
            <LinkContainer to="/todostype">
              <Nav.Link className="nav-bar-link-color">
                Táblák kezelése
              </Nav.Link>
            </LinkContainer>
          </Nav>
          <Navbar.Text className="color">
            Kálmán Richárd (JSRFDB)
          </Navbar.Text>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default NavBarComponent;
