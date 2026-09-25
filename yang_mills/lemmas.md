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
PROVED under the stated convergence/reflection-positivity hypotheses.

Proof:
At cutoff a_k (and after a volume-uniform thermodynamic estimate), reflection positivity gives a transfer Hamiltonian H_k. The common lattice-unit exponent gamma(a_k) excludes transfer spectrum below gamma(a_k), so if gamma(a_k)/a_k >= m_* then for every centered local vector psi_k,
C_k(t_k)=<psi_k,exp(-t_k H_k)psi_k> <= exp(-m_* t_k) C_k(0)
at lattice times t_k=n_k a_k.
For fixed physical t choose n_k=floor(t/a_k), hence t_k->t. By convergence of the renormalized diagonal Schwinger functions and their t=0 norms,
C(t) <= exp(-m_* t) C(0).
OS reconstruction identifies C(t)=<psi,exp(-tH)psi> in the continuum. YM-BRIDGE-1 then excludes continuum spectrum in (0,m_*), provided these local vectors are dense in Omega^perp.

Gap:
The theorem is conditional on the actual Yang-Mills hypotheses: cutoff/volume-uniform gamma(a)>=m_* a, convergence of renormalized local correlators including norms, and OS reconstruction. Those are not proved by this bridge.

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


---

## LEMMA ID: YM-SEMIGROUP-1
Statement:
Let mu be a Gibbs measure on a bounded-degree lattice, invariant for a reversible diffusion semigroup P_t with generator L. Assume:

(1) Poincare decay:
Var_mu(P_t f) <= exp(-2 lambda t) Var_mu(f).

(2) Diffusion/carre-du-champ identity:
P_t(fg)-P_t f P_t g
= 2 integral_0^t P_s Gamma(P_{t-s}f,P_{t-s}g) ds,
with Gamma decomposing into local edge gradients.

(3) Weighted gradient propagation: for every kappa>0 in some interval and local f supported on A,
G_e(P_t f) <= L_f exp(v_kappa t-kappa d(e,A)),
where G_e is the local gradient norm and v_kappa is independent of volume.

Then for local f,g with supports A,B and R=d(A,B),
|Cov_mu(f,g)| <= C_{f,g,kappa} exp(-gamma R),
with a common rate
gamma >= kappa lambda/[2(lambda+v_kappa)]
(up to an arbitrarily small loss if the underlying graph only has polynomial volume growth).

If the Euclidean lattice measure is reflection positive, the physical transfer-Hamiltonian gap in lattice units is at least this common clustering rate on the dense local gauge-invariant sector.

Purpose:
Separate the two ingredients hidden inside “functional inequality implies mass gap”: relaxation in auxiliary Markov time and finite propagation in Euclidean space.

Dependencies:
Reversible diffusion calculus; reflection positivity only for the last spectral interpretation.

Status:
PROVED under the stated abstract hypotheses.

Proof:
By invariance,
Cov(f,g)=mu(P_t(fg)-P_t f P_t g)+Cov(P_t f,P_t g).
The Poincare assumption and Cauchy-Schwarz give
|Cov(P_t f,P_t g)| <= exp(-2 lambda t) ||f-mu f||_2 ||g-mu g||_2.
For the first term, the diffusion identity and weighted gradient bound give
||P_t(fg)-P_t f P_t g||_infty
<= C L_f L_g exp(-kappa R/2+2v_kappa t).
Here one uses
sum_e exp[-kappa(d(e,A)+d(e,B))]
<= C_{kappa,d}|A||B| exp(-kappa R/2)
on a polynomial-growth lattice. Choosing
t = kappa R/[4(lambda+v_kappa)]
makes both exponents at least
kappa lambda R/[2(lambda+v_kappa)].
This proves the claim.

Gap:
At weak coupling, no required cutoff-sharp lower bound on lambda/(lambda+v_kappa) has been proved here.

Counterexample search:
A global Poincare constant may be unnecessarily strong and may be degraded by slow topological/global modes even if local physical correlations are short-ranged. Therefore the next version should seek a conditional/local or observable-restricted replacement.

---

## LEMMA ID: YM-RG-LANDING-1
Statement:
Fix a block factor b>1. Suppose an exact gauge-invariant RG can be iterated to a scale k_*(a) such that the coarse spacing
a_* = b^{k_*(a)} a
obeys c_1/Lambda_YM <= a_* <= c_2/Lambda_YM, and the exact effective measure admits a polymer representation with activities z(X) satisfying a volume- and cutoff-independent Kotecky-Preiss type bound
sup_B sum_{X contains B} ||z(X)|| exp(alpha |X| + mu diam(X)) <= epsilon < epsilon_KP
for fixed alpha,mu>0. Assume the exact RG map sends microscopic local gauge-invariant observables to quasi-local coarse observables whose tails decay faster than the same polymer rate.

Then the original lattice theory has exponential gauge-invariant correlation decay with physical rate at least c Lambda_YM, and, using reflection positivity at the ORIGINAL Wilson lattice level, its transfer Hamiltonian has a physical gap >= c Lambda_YM.

Purpose:
Turn the vague slogan “RG flows to strong coupling” into one precise landing inequality in a known mixing domain.

Dependencies:
Standard convergent polymer/cluster expansion once the KP bound holds; exact pullback of observables; YM-BRIDGE-1 / lattice transfer spectral argument.

Status:
PARTIALLY PROVED.

Proof:
The KP bound yields exponential connected-correlation decay exp(-mu R_*) in coarse lattice units for quasi-local observables. Pulling the estimate back through the exact RG gives microscopic decay exp[-c mu r/a_*]. Since a_* is comparable to Lambda_YM^{-1}, the physical decay exponent is >= c' Lambda_YM. Reflection positivity is required only for the original Wilson measure; the blocked effective action itself need not be reflection positive. The spectral theorem then gives the transfer gap.

Gap:
The landing hypothesis itself: prove that 4D asymptotically-free Yang-Mills enters such a uniform polymer domain at a physical scale O(Lambda_YM^{-1}). This is precisely the UV-to-IR crossover not controlled by present perturbative/strong-coupling estimates.

Counterexample search:
Merely reaching an O(1) effective coupling is not enough; the KP norm must actually be below its convergence threshold with cutoff-independent constants.


---

## LEMMA ID: YM-CROSSOVER-1
Statement:
Fix a block factor b>1 and a Banach space B of gauge-invariant effective interactions with an exact RG map R. Assume the UV construction supplies, at the first matching scale where a chosen renormalized coupling reaches a fixed small value g_match>0, a cutoff- and volume-uniform compact set K_match subset B of possible effective actions, with matching spacing a_match comparable (up to a fixed G-dependent factor) to Lambda_YM^{-1}.

Prove that there exist an integer M_G<infinity and q<1, both independent of the original cutoff a and volume, such that
R^{M_G}(K_match) subset D_KP(q),
where D_KP(q) is a rigorously specified strong-mixing/polymer domain satisfying a Kotecky-Preiss exponential norm bound.

Purpose:
Reduce the nonperturbative UV-to-IR crossover to a FINITE number of RG steps independent of the UV cutoff. Once this inclusion holds, YM-RG-LANDING-1 follows.

Dependencies:
A UV RG package that reaches K_match uniformly and an exact RG map with controlled observable pullback.

Status:
CONJECTURED.

Proof:
No proof of the inclusion. The reduction from this inclusion to a gap is proved by YM-RG-LANDING-1.

Why finite M is plausible but not proved:
The cutoff-dependent number of RG steps occurs only while running from g_0(a)->0 to the fixed g_match. After g_match is fixed, going to any other fixed strong-coupling threshold spans a fixed logarithmic scale interval, hence a fixed number of b-blocks in the RG heuristic. Establishing the corresponding nonperturbative inclusion in the full effective-action space is exactly the missing step.

Gap:
Construct a concrete B, K_match and D_KP and validate the finite-step inclusion without projecting away relevant/irrelevant interactions or assuming monotonic coupling flow.

Counterexample search:
Tracking a single scalar coupling is insufficient: irrelevant operators generated by RG can leave the strong-mixing domain even when a nominal effective coupling is “large”. The lemma must control the full interaction norm.
