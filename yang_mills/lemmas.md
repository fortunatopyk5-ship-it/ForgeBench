# Lemmas

## LEMMA ID: YM-BRIDGE-1
Statement:
Let (H,Omega,Hamiltonian H) be an OS-reconstructed theory with H>=0. Suppose a centered local gauge-invariant algebra produces a dense set D in Omega^perp. Assume there is a common m_*>0 such that for every psi in D there is C_psi<infinity with
<psi,e^{-tH}psi> <= C_psi e^{-m_* t}
for all sufficiently large t.
Then Spec(H)∩(0,m_*)=empty.

Purpose:
Turn Euclidean-time clustering into the spectral mass gap without treating that implication as automatic.

Dependencies:
OS reconstruction; density/cyclicity of the chosen centered algebra.

Status:
PROVED.

Proof:
For psi, spectral theorem gives
<psi,e^{-tH}psi> = integral exp(-tE) dmu_psi(E).
If mu_psi([0,m_*-epsilon])>0 for some epsilon>0, the LHS is at least
mu_psi([0,m_*-epsilon]) exp(-(m_*-epsilon)t),
contradicting the assumed upper bound as t->infinity.
Thus mu_psi([0,m_*))=0 for all psi in D. The spectral projection
P=1_[0,m_*)(H) is bounded and vanishes on dense D subset Omega^perp, hence P|Omega^perp=0.
Centering removes the vacuum atom at E=0.

Gap:
Need the hypotheses from an actual continuum Yang–Mills construction.

Counterexample search:
Spatial clustering without OS/time-translation reconstruction is not sufficient; the proof specifically uses a positive Euclidean-time semigroup.

---

## LEMMA ID: YM-CUTOFF-BRIDGE-1
Statement:
Let a_k->0 be reflection-positive lattice theories whose renormalized gauge-invariant Schwinger functions converge to an OS-reconstructible limit. If for a dense generating family the diagonal Euclidean-time correlators have a common lattice-unit decay exponent gamma(a_k) with
liminf gamma(a_k)/a_k >= m_*>0,
and the correlators converge with their t=0 norms, then the continuum Hamiltonian has gap at least m_*.

Purpose:
State the exact cutoff scaling needed from a lattice estimate.

Dependencies:
reflection positivity, convergence of renormalized correlators, YM-BRIDGE-1.

Status:
PARTIALLY PROVED.

Proof:
At each cutoff, RP gives a positive transfer semigroup. The common decay exponent excludes lattice transfer spectrum below gamma(a_k). Under convergence of diagonal Laplace transforms plus t=0 mass control, spectral measures cannot acquire support below m_* in the weak limit. Apply YM-BRIDGE-1 to the limiting dense algebra.

Gap:
Write a fully abstract varying-Hilbert-space spectral-measure convergence theorem and verify the observable-renormalization hypotheses in the YM setting.

Counterexample search:
If only gamma(a)>0 is known, gamma(a)/a may tend to zero. Fixed-cutoff positivity therefore does not imply a continuum mass gap.

---

## LEMMA ID: YM-IR-1
Statement:
Along a continuum Wilson trajectory beta=beta(a)->infinity, prove for some c_G>0
gamma(a) >= c_G a Lambda_YM
uniformly in volume, where gamma(a) is a common Euclidean-time exponential clustering rate in lattice units for a dense gauge-invariant local algebra.

Purpose:
This is the minimal IR estimate needed after continuum construction.

Dependencies:
none beyond the regulated lattice theory and a chosen continuum trajectory.

Status:
CONJECTURED.

Proof:
None.

Gap:
Weak-coupling, volume-uniform, cutoff-sharp correlation decay.

Counterexample search:
Any estimate gamma(a)>=exp(-C beta) is insufficient unless C and logarithmic/power corrections are quantitatively sharp relative to the RG scaling of a Lambda_YM.

---

## LEMMA ID: YM-FI-1
Statement:
For a local reversible stochastic-quantization generator L_a with invariant Wilson measure, let lambda_a be a Poincare/spectral relaxation rate and v_a a finite-range derivative-propagation scale obtained from commutators [nabla_e,L_a]. Prove a normalization-independent bound
lambda_a/v_a >= c_G a Lambda_YM
along the continuum trajectory, together with uniform locality constants sufficient to convert this ratio into Euclidean covariance decay.

Purpose:
A more attackable functional-inequality proxy for YM-IR-1.

Dependencies:
A Shen–Zhu–Zhu/Guionnet–Zegarlinski type semigroup-to-spatial-mixing theorem with cutoff-explicit constants.

Status:
CONJECTURED.

Proof:
At strong coupling an analogous mechanism is rigorous. The ratio is invariant under an overall rescaling L_a -> r_a L_a, unlike lambda_a alone.

Gap:
No weak-coupling lower bound with the required physical scaling is known here.

Counterexample search:
Bakry–Emery curvature gives no continuation: for SU(N) in d dimensions the known strong-coupling constant is
K_S=N(1/2-8|beta|(d-1)),
so in d=4 it is positive only for |beta|<1/48 and becomes negative in the continuum regime beta->infinity.

---

## LEMMA ID: YM-UV-1
Statement:
Construct a nontrivial continuum limit of 4D Wilson lattice Yang–Mills along an asymptotically-free trajectory, with renormalized gauge-invariant Schwinger functions satisfying corrected Osterwalder–Schrader hypotheses strongly enough for Wightman reconstruction.

Purpose:
Existence half of the Millennium problem.

Dependencies:
constructive RG, UV stability, thermodynamic limit, observable renormalization, Euclidean covariance restoration.

Status:
UNKNOWN.

Proof:
None in this notebook.

Gap:
Balaban-type UV control does not by itself supply the full R^4 OS theory with the IR limit and mass gap.

Counterexample search:
A subsequential or fixed-volume UV limit is insufficient if thermodynamic-limit control or nontriviality is missing.
